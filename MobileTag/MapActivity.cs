#pragma warning disable CS0618 // Suppress warning: Type or member is obsolete

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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", LaunchMode = LaunchMode.SingleTask)]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Locations.ILocationListener, GoogleMap.IOnCameraIdleListener
    {
        private const double CELL_LOAD_RADIUS = .0006;
        private const int ZOOM_LEVEL_LOAD = 15;
        private GoogleMap mMap;
        public GoogleMap Map => mMap;
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

        private System.Timers.Timer timer = new System.Timers.Timer();


        ConcurrentDictionary<int, MapOverlay> OverlaysToDraw = new ConcurrentDictionary<int, MapOverlay>();

        private bool doubleBackToExitPressedOnce = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            SetUpUI();           

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

            //Drawer navigation menu event handler
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_profile:
                        StartActivity(new Intent(this, typeof(ProfileActivity)));
                        break;
                    case Resource.Id.nav_map:
                        // we are already in map activity
                        break;
                    case Resource.Id.nav_settings:                        
                        StartActivity(new Intent(this, typeof(SettingsActivity)));                       
                        break;
                    case Resource.Id.nav_logout:
                        GameModel.Logout();
                        this.Finish(); 
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    default:
                        break;
                }               
                drawerLayout.CloseDrawers();
              
            };           
            await cellHubSetupTask;
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void SetUpUI()
        {
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
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            // Example code for map style: https://developers.google.com/maps/documentation/android-api/styling
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, GameModel.MapStyle));
            mMap = googleMap;
            mMap.UiSettings.ZoomControlsEnabled = true;
            mMap.SetOnCameraIdleListener(this);
            mMap.MapClick += MMap_MapClick;
        }

        private void MMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            int clickedCellID = Cell.FindID(e.Point);

            if (GameModel.CellsInView.ContainsKey(clickedCellID))
            {
                Cell cell = GameModel.CellsInView[clickedCellID];

                if (cell.MapOverlay.CellIsOnMap)
                {
                    cell.MapOverlay.Click(this);   
                }
            }
        }

        public async void PlantMinePrompt()
        {           
            try
            {   
                var id = Cell.FindID((decimal)mMap.MyLocation.Latitude, (decimal)mMap.MyLocation.Longitude);
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialogCellInfo cellInfoDialog = new dialogCellInfo(await Database.GetCell(id));
                cellInfoDialog.Show(transaction, "Dialog Fragment");
            }
            catch (Exception ex)
            {
                string exString = "PlantMinePrompt exception" + ex.ToString();
                Toast.MakeText(this, exString, ToastLength.Long).Show();
            }

        }

        public async void PlantAntiMinePrompt()
        {
            try
            {
                var id = Cell.FindID((decimal)mMap.MyLocation.Latitude, (decimal)mMap.MyLocation.Longitude);
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                EnemyDialogCell cellInfoDialog = new EnemyDialogCell(await Database.GetCell(id));
                cellInfoDialog.Show(transaction, "Dialog Fragment");
            }
            catch (Exception ex)
            {
                string exString = "PlantAntiMinePrompt exception" + ex.ToString();
                Toast.MakeText(this, exString, ToastLength.Long).Show();
            }
        }

        private void DrawerLayout_DrawerStateChanged(object sender, DrawerLayout.DrawerStateChangedEventArgs e)
        {
            TextView usernameHeader = FindViewById<TextView>(Resource.Id.nameTxt);
            TextView userConfinium = FindViewById<TextView>(Resource.Id.confiniumTxt);
            usernameHeader.Text = GameModel.Player.Username;
            userConfinium.Text = GameModel.Player.Wallet.Confinium.ToString();
        }

        //Hamburger Button Pressed
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);

            return base.OnOptionsItemSelected(item);
        }

        protected override async void OnResume()
        {
            base.OnResume();            


            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                RequestLocationUpdates();
            }
            if (CellHub.Connection.State != ConnectionState.Connected && CellHub.Connection.State != ConnectionState.Connecting)
            {
                await CellHub.Connection.Start();
            }

            if (mMap != null)
                mMap.Clear();

            OverlaysToDraw.Clear();
            GameModel.CellsInView.Clear();
            GameModel.Player.AntiMines.Clear();
            GameModel.Player.Mines.Clear();

            // Get fresh player data
            GameModel.Player.AntiMines = await Database.GetAntiMines(GameModel.Player.ID);
            GameModel.Player.Mines = await Database.GetMines(GameModel.Player.ID);

            if (initialCameraLatLng != null)
            {
                // Refresh stale cell data
                await DrawCellsInView();
            }
        }

       public override void OnBackPressed()
        {

            if (doubleBackToExitPressedOnce)
            {
                //base.OnBackPressed();
                Java.Lang.JavaSystem.Exit(0);
                return;
            }


            this.doubleBackToExitPressedOnce = true;
            Toast.MakeText(this, "Press back again to exit", ToastLength.Short).Show();

            new Handler().PostDelayed(() => {
                doubleBackToExitPressedOnce = false;
            }, 2000);
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                if (locMgr != null)
                    locMgr.RemoveUpdates(this);
            }

            CellHub.Connection.Stop();
        }

        private void CenterMapCameraOnLocation()
        {
            if (mMap.MyLocation != null)
            {
                CameraUpdate mapCameraPos = CameraUpdateFactory.NewLatLngZoom(new LatLng(mMap.MyLocation.Latitude, mMap.MyLocation.Longitude), 17);
                mMap.AnimateCamera(mapCameraPos);
            }
        }

        private async void LocationButton_Click(object sender, EventArgs e)
        {
            int totalYield = 0;

            foreach (Mine mine in GameModel.Player.Mines.Values)
            {
                totalYield += await mine.Yield();
                
            }

            await GameModel.Player.Wallet.AddConfinium(totalYield);

            Toast.MakeText(this, "Yielded " + totalYield + " confinium.", ToastLength.Long).Show();
        }

        private void RequestLocationUpdates()
        {
            if (locMgr != null)
            {
                locMgr.RequestLocationUpdates(LocationManager.GpsProvider, 10000, 2, this);
                locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 10000, 2, this);
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

            RequestLocationUpdates();

            if (lastKnownLocation == null)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't find location");
                DisplayStatus("Couldn't find location");
            }
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

                    if (GameModel.CellsInView.ContainsKey(updatedCell.ID))
                    {
                        // Set updateCell's MapOverlay to existing MapOverlay so we don't lose that reference and draw on top of the lost overlay
                        updatedCell.MapOverlay = GameModel.CellsInView[updatedCell.ID].MapOverlay;

                        GameModel.CellsInView[updatedCell.ID] = updatedCell;
                    }
                    else
                    {
                        GameModel.CellsInView.TryAdd(updatedCell.ID, updatedCell);
                    }

                    if (updatedCell.TeamID != GameModel.Player.Team.ID && GameModel.Player.Mines.ContainsKey(updatedCell.ID))
                    {
                        RunOnUiThread(() =>
                        {
                            // remove player's mine in this cell
                            GameModel.Player.RemoveMine(updatedCell.ID);
                        });
                    }

                    if (updatedCell.TeamID == GameModel.Player.Team.ID && GameModel.Player.AntiMines.ContainsKey(updatedCell.ID))
                    {
                        RunOnUiThread(() =>
                        {
                            // remove player's antimine in this cell
                            GameModel.Player.RemoveAntiMine(updatedCell.ID);
                        });
                    }

                    UpdateOverlay(updatedCell);
                });

                await CellHub.Connection.Start();

                GameModel.Player.StartAntiMines(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // -------------------------------------------------------------------------------------------------

        
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

        public void DisplayCellInfo(int cellID)
        {
            throw new NotImplementedException();
        }

        public void DisplayStatus(string status)
        {
            statusText.Text = status;
        }

        public void DisplayStatus(string status, double length)
        {
            statusText.Text = status;

            timer.Stop();
            timer = new System.Timers.Timer(length);
            timer.AutoReset = false;

            timer.Elapsed += (o, e) => {
                RunOnUiThread(() => ClearStatus());
                timer.Stop();
            };

            timer.Start();
        }

        private void ClearStatus()
        {
            if (timer.Enabled == false)
            {
                statusText.Text = "";
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
                int playerCellID = Cell.FindID(decLat, decLng);
                Cell cell;

                if (!GameModel.CellsInView.ContainsKey(playerCellID))
                {
                    // Generate the new cell and add it to CellsInView
                    cell = new Cell(decLat, decLng);
                    GameModel.CellsInView.TryAdd(cell.ID, cell);
                    await CellHub.SubscribeToCellUpdates(cell.ID);
                }
                else
                {
                    cell = GameModel.CellsInView[playerCellID];
                }

                try
                {
                    if (!cell.MapOverlay.CellIsOnMap)
                        cell.MapOverlay.Draw(mMap);

                    await cell.Tag(); 
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

        private async Task DrawCellsInView()
        {
            DisplayStatus("Loading new cells...");

            await Task.Run(async () =>
            {
                // We want to add newly created overlays while retaining all previously existing Polygon references in CellsInView
                await GameModel.LoadProximalCells(initialCameraLatLng);

                foreach (Cell cell in GameModel.CellsInView.Values)
                {
                    if (cell.TeamID > 0 && !cell.MapOverlay.CellIsOnMap)
                    {
                        OverlaysToDraw.TryAdd(cell.MapOverlay.CellID, cell.MapOverlay);
                    }
                }
            });

            await DrawOverlays();

            ClearStatus();
        }

        public void UpdateOverlay(Cell updatedCell)
        {
            RunOnUiThread(() =>
            {
                updatedCell.MapOverlay.UpdateColor(updatedCell.HoldStrength, updatedCell.TeamID);

                if (!updatedCell.MapOverlay.CellIsOnMap)
                {
                    // Cell isn't on the map yet... draw it!
                    updatedCell.MapOverlay.Draw(mMap);
                }
            });
        }

        private async Task DrawOverlays()
        {
            foreach (Cell cell in GameModel.CellsInView.Values)
            {
                if (OverlaysToDraw.ContainsKey(cell.MapOverlay.CellID))
                {
                    cell.MapOverlay.Draw(mMap);
                    OverlaysToDraw.TryRemove(cell.MapOverlay.CellID, out MapOverlay value);
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