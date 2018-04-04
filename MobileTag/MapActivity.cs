using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Support.V4.App;
using Android.Content.PM;
using MobileTag.Models;
using Microsoft.AspNet.SignalR.Client;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using MobileTag.SharedCode;
using System.Collections.Concurrent;
using Android.Support.V7.App;
using System.Threading.Tasks;

namespace MobileTag
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Locations.ILocationListener, GoogleMap.IOnCameraIdleListener
    {
        private const double CELL_LOAD_RADIUS = .0006;
        private const int ZOOM_LEVEL_LOAD = 15;
        private GoogleMap mMap;
        private float currentZoomLevel = 0.0F;
        private LocationManager locMgr;
        private String provider;
        private bool InitialCameraLocSet = false;
        private LatLng initialCameraLatLng = null;
        private bool locationFound = false;

        private Location lastKnownLocation;
        private TextView statusText;
        private Button tagButton;
        private Button locationButton;
        readonly string[] LocationPermissions = { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation };
        private const int RequestLocationID = 0;

        private DrawerLayout drawerLayout;
        private NavigationView navigationView;

        ConcurrentDictionary<int, MapOverlay> Overlays = new ConcurrentDictionary<int, MapOverlay>();
        ConcurrentDictionary<int, MapOverlay> OverlaysToDraw = new ConcurrentDictionary<int, MapOverlay>();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);
         
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            toolbar.SetBackgroundColor(ColorCode.TeamColor(GameModel.Player.Team.ID));

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            statusText = FindViewById<TextView>(Resource.Id.textBelowMap);
            tagButton = FindViewById<Button>(Resource.Id.claimButton);
            locationButton = FindViewById<Button>(Resource.Id.clientCameraLocationbtn);

            ActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_dehaze_white_24dp);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout.DrawerStateChanged += DrawerLayout_DrawerStateChanged;

            tagButton.Click += TagButton_Click;
            locationButton.Click += LocationButton_Click;

            SetUpMap();

            var cellHubSetupTask = SetUpCellHub();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                DisplayStatus("Finding location...");
                GetLocation();
            }
            else
            {
                RequestPermissions(LocationPermissions, RequestLocationID);
            }

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_logout:
                        GameModel.Logout();
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawers();
            };

            await cellHubSetupTask;
        }

        private void DrawerLayout_DrawerStateChanged(object sender, DrawerLayout.DrawerStateChangedEventArgs e)
        {
            TextView usernameHeader = FindViewById<TextView>(Resource.Id.nameTxt);
            usernameHeader.Text = GameModel.Player.Username;
        }

        protected override async void OnResume()
        {
            base.OnResume();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                locMgr.RequestLocationUpdates(LocationManager.GpsProvider, 10000, 10, this);
                locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 10000, 10, this);
            }
            if (CellHub.Connection.State != ConnectionState.Connected && CellHub.Connection.State != ConnectionState.Connecting)
            {
                await CellHub.Connection.Start();

                if (initialCameraLatLng != null)
                {
                    // Refresh stale cell data
                    mMap.Clear();
                    Overlays.Clear();
                    OverlaysToDraw.Clear();
                    GameModel.CellsInView.Clear();

                    await DrawCellsInView();
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                locMgr.RemoveUpdates(this);
            }

            CellHub.Connection.Stop();
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);

            return base.OnOptionsItemSelected(item);
        }


        // SignalR------------------------------------------------------------------------------------------

        // Start the CellHub SignalR connection
        private async Task SetUpCellHub()
        {
            try
            {
                CellHub.HubProxy.On<Cell>("broadcastCell", updatedCell =>
                {
                    // Handle SignalR cell update notification
                    Console.WriteLine("Cell {0} updated!", updatedCell.ID);
                    GameModel.CellsInView[updatedCell.ID] = updatedCell;
                    UpdateOverlay(updatedCell);
                });

                await CellHub.Connection.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            // Example code for map style: https://developers.google.com/maps/documentation/android-api/styling
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.style_json));
            mMap = googleMap;
            mMap.UiSettings.ZoomControlsEnabled = true;
            mMap.SetOnCameraIdleListener(this);
        }

        public async void OnCameraIdle()
        {
            currentZoomLevel = mMap.CameraPosition.Zoom;
            LatLng currentCameraLatLng = mMap.CameraPosition.Target;

            if (currentZoomLevel > ZOOM_LEVEL_LOAD)
            {
                if (InitialCameraLocSet == false)
                {
                    initialCameraLatLng = mMap.CameraPosition.Target;
                    InitialCameraLocSet = true;

                    await DrawCellsInView();
                }
                else
                {
                    double distanceFromInitialCameraPosition = Math.Sqrt(Math.Pow(currentCameraLatLng.Latitude - initialCameraLatLng.Latitude, 2) + Math.Pow(currentCameraLatLng.Longitude - initialCameraLatLng.Longitude, 2));

                    if (distanceFromInitialCameraPosition > CELL_LOAD_RADIUS)
                    {
                        initialCameraLatLng = mMap.CameraPosition.Target;

                        await DrawCellsInView();
                    }
                }
            }
        }

        private async Task DrawCellsInView()
        {
            DisplayStatus("Loading new cells...");

            await Task.Run(async () =>
            {
                if (Overlays.IsEmpty)
                {
                    Overlays = await GameModel.LoadProximalCells(initialCameraLatLng);

                    foreach (MapOverlay overlay in Overlays.Values)
                    {
                        OverlaysToDraw.TryAdd(overlay.CellID, overlay);
                    }
                }
                else
                {
                    // We want to add newly created overlays while retaining all previously existing Polygon references in Overlays
                    ConcurrentDictionary<int, MapOverlay> temp = await GameModel.LoadProximalCells(initialCameraLatLng);

                    foreach (MapOverlay mOverlay in temp.Values)
                    {
                        if (!Overlays.Keys.Contains(mOverlay.CellID))
                        {
                            // Add the new overlay to Overlays
                            Overlays.TryAdd(mOverlay.CellID, mOverlay);
                            OverlaysToDraw.TryAdd(mOverlay.CellID, mOverlay);
                        }
                    }
                }
            });
            
            await DrawOverlays();

            ClearStatus();
        }

        public void OnLocationChanged(Location location)
        {
            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                if (mMap.MyLocationEnabled != true)
                {
                    mMap.MyLocationEnabled = true;
                }

                if (location == null)
                {
                    System.Diagnostics.Debug.WriteLine("No Location");
                    locationFound = false;
                    DisplayStatus("Couldn't find location");
                }
                else if (locationFound == false)
                {
                    locationFound = true;
                    DisplayStatus("Location found!", 5000);
                }

                if (location != null)
                {
                    lastKnownLocation = location;
                }
            }
        }

        // Based on example code from https://blog.xamarin.com/requesting-runtime-permissions-in-android-marshmallow/
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            GetLocation();
                        }
                        else
                        {                            
                            StartActivity(new Intent(this, typeof(LoginActivity)));
                        }
                    }

                    break;
            }
        }

        private void GetLocation()
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;

            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Fine;
            locationCriteria.PowerRequirement = Power.Medium;
            provider = locMgr.GetBestProvider(locationCriteria, true);
            lastKnownLocation = locMgr.GetLastKnownLocation(provider);

            if (lastKnownLocation == null)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't find location");
                DisplayStatus("Couldn't find location");
            }
        }

        private void DisplayStatus(string status)
        {
            statusText.Text = status;
        }

        private void DisplayStatus(string status, double length)
        {
            statusText.Text = status;

            System.Timers.Timer timer = new System.Timers.Timer(length);
            timer.AutoReset = false;

            timer.Elapsed += (o, e) => {
                RunOnUiThread(() => ClearStatus());
                timer.Stop();
            };

            timer.Start();
        }

        private void ClearStatus()
        {
            statusText.Text = "";
        }

        private void CenterMapCameraOnLocation()
        {
            if (mMap.MyLocation != null)
            {
                CameraUpdate mapCameraPos = CameraUpdateFactory.NewLatLngZoom(new LatLng(mMap.MyLocation.Latitude, mMap.MyLocation.Longitude), 17);
                mMap.AnimateCamera(mapCameraPos);
            }
        }

        private void LocationButton_Click(object sender, EventArgs e)
        {
            if (locationFound == true)
            {
                CenterMapCameraOnLocation();
            }
            else
            {
                Toast.MakeText(this, "Location unknown...", ToastLength.Long).Show();
            }
        }

        private async void TagButton_Click(object sender, EventArgs e)
        {
            if (locationFound == false)
            {
                Toast.MakeText(this, "Tag failed... location unknown", ToastLength.Long).Show();
            }
            else
            {
                decimal decLat = (decimal)(mMap.MyLocation.Latitude);
                decimal decLng = (decimal)(mMap.MyLocation.Longitude);
                int playerCellID = GameModel.GetCellID(decLat, decLng);
                Cell cell;

                if (!GameModel.CellsInView.ContainsKey(playerCellID))
                {
                    // Generate the new cell and add it to CellsInView
                    cell = GameModel.GenerateCell(decLat, decLng);
                    GameModel.CellsInView.TryAdd(cell.ID, cell);
                    await CellHub.SubscribeToUpdates(cell.ID);
                }
                else
                {
                    cell = GameModel.CellsInView[playerCellID];
                }

                try
                {
                    var tagTask = cell.Tag();
                    UpdateOverlay(cell);
                    await tagTask;
                }
                catch (AggregateException exc)
                {
                    foreach (Exception ie in exc.InnerExceptions)
                        Console.WriteLine(ie.ToString());
                }
                catch (Exception o)
                {
                    Console.WriteLine(o.ToString());
                }
            }
        }

        public void UpdateOverlay(Cell updatedCell)
        {
            if (!Overlays.ContainsKey(updatedCell.ID))
            {
                RunOnUiThread(() =>
                {
                    MapOverlay mapOverlay = new MapOverlay(updatedCell);

                    mapOverlay.Draw(mMap);

                    Overlays.TryAdd(updatedCell.ID, mapOverlay);
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Overlays[updatedCell.ID].SetColor(ColorCode.TeamColor(updatedCell.TeamID));
                });
            }
        }

        private async Task DrawOverlays()
        {
            foreach (MapOverlay Overlay in Overlays.Values)
            {
                if (OverlaysToDraw.Keys.Contains(Overlay.CellID))
                {
                    Overlay.Draw(mMap);
                    OverlaysToDraw.TryRemove(Overlay.CellID, out MapOverlay value);
                    await Task.Delay(40); // Bad practice... but this delay frees up the UI thread for a bit to respond to user input (e.g. map movement)
                }
            }
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }
}