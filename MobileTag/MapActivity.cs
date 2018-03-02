using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Support.V4.App;
using Android.Graphics;
using Android.Content.PM;
using Android.Gms.Tasks;
using MobileTag.Models;
using Microsoft.AspNet.SignalR.Client;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using MobileTag.SharedCode;
using System.Collections.Concurrent;
using Android.Support.V7.App;

namespace MobileTag
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Locations.ILocationListener, GoogleMap.IOnCameraIdleListener, GoogleMap.IOnCameraMoveStartedListener, GoogleMap.IOnCameraMoveListener
    {
        private const double CELL_LOAD_RADIUS = .002;
        private GoogleMap mMap;
        private float currentZoomLevel = 0.0F;
        private LocationManager locMgr;
        private String provider;
        private bool InitialCameraLocSet = false;
        private LatLng initialCameraLatLng = null;

        private Location lastKnownLocation;
        private Marker myPositionMarker;
        private TextView lngLatText;
        private Button tagButton;
        private Button locationButton;
        readonly string[] LocationPermissions = { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation };
        private const int RequestLocationID = 0;

        private DrawerLayout drawerLayout;
        private NavigationView navigationView;

        ConcurrentDictionary<int, MapOverlay> Overlays;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ////Connects Map.axml to this Activity
            SetContentView(Resource.Layout.Map);
         
            //Bind C# objects to xml elements
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            lngLatText = FindViewById<TextView>(Resource.Id.textBelowMap);
            tagButton = FindViewById<Button>(Resource.Id.claimButton);
            locationButton = FindViewById<Button>(Resource.Id.clientCameraLocationbtn);

            //Enable toolbar to display hamburger
            ActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_black_24dp);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            
            //click events           
            tagButton.Click += TagButton_Click;
            locationButton.Click += LocationButton_Click;

            SetUpMap();
            SetUpCellHub();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                // Location permissions have been granted
                GetLocation();
            }
            else
            {
                RequestPermissions(LocationPermissions, RequestLocationID);
            }

            //menu item selected
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                //react to click here and swap fragments or navigate               
                drawerLayout.CloseDrawers();
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                locMgr = GetSystemService(Context.LocationService) as LocationManager;
                locMgr.RequestLocationUpdates(LocationManager.GpsProvider, 10000, 10, this);
                locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 10000, 10, this);
            }

            // Connect to cellHub if we aren't already connected
            if (GameModel.CellHubConnection.State != ConnectionState.Connected && GameModel.CellHubConnection.State != ConnectionState.Connecting)
            {
                GameModel.CellHubConnection.Start().Wait();
                GameModel.SubscribeToUpdates();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                ////This will remove the listener from constantly grabbing locations
                locMgr.RemoveUpdates(this);
            }

            // End the current connection to the SignalR server
            GameModel.CellHubConnection.Stop();
        }

        //tells drawer to open when hamburger button is pressed        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


        // SignalR------------------------------------------------------------------------------------------

        // Set up the cellHub proxy and start the connection. 
        private void SetUpCellHub()
        {
            try
            {
                GameModel.CellHubProxy = GameModel.CellHubConnection.CreateHubProxy("cellHub");

                GameModel.CellHubProxy.On<Cell>("broadcastCell", updatedCell =>
                {
                    // Handle SignalR cell update notification
                    Console.WriteLine("Cell {0} updated!", updatedCell.ID);
                    GameModel.CellsInView[updatedCell.ID] = updatedCell;
                    UpdateOverlay(updatedCell);
                });

                GameModel.CellHubConnection.Start().Wait();
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

        private void MMap_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 10));
            Console.WriteLine(pos.ToString());
        }

        /*This function is called from SetUpMap()*/
        public void OnMapReady(GoogleMap googleMap)
        {
            // Example code for map style: https://developers.google.com/maps/documentation/android-api/styling
            googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.style_json)); // We may want to wrap this in a try-catch block

            ////Map Creation
            mMap = googleMap;
            mMap.UiSettings.ZoomControlsEnabled = true;
            mMap.SetOnCameraIdleListener(this);

            ////Event after marker finishes being dragged
            mMap.MarkerDragEnd += MMap_MarkerDragEnd;

        }

        public void OnCameraIdle()  //part of OnCameraIdleListener Interface
        {
            //work in progress
            currentZoomLevel = mMap.CameraPosition.Zoom;
            LatLng currentCameraLatLng = mMap.CameraPosition.Target;

            if (currentZoomLevel > 15)
            {
                if (InitialCameraLocSet == false)
                {
                    // We don't have an initial camera location. Set that now.
                    initialCameraLatLng = mMap.CameraPosition.Target;
                    Overlays = GameModel.LoadProximalCells(initialCameraLatLng);
                    DrawOverlays();
                    InitialCameraLocSet = true;
                }
                else
                {
                    // Measure the distance between the initial camera position and the current camera position
                    double distanceFromInitialCameraPosition = Math.Sqrt(Math.Pow(currentCameraLatLng.Latitude - initialCameraLatLng.Latitude, 2) + Math.Pow(currentCameraLatLng.Longitude - initialCameraLatLng.Longitude, 2));

                    if (distanceFromInitialCameraPosition > CELL_LOAD_RADIUS)
                    {
                        initialCameraLatLng = mMap.CameraPosition.Target;

                        // Load new cells
                        Toast.MakeText(this, "Loading new cells: " + currentZoomLevel.ToString(), ToastLength.Long).Show();

                        // We most likely should add code to load the cells in view here... LoadProximalCells could work?
                        Overlays = GameModel.LoadProximalCells(currentCameraLatLng);
                        DrawOverlays();
                    }
                }
            }
        }

        public void OnLocationChanged(Location location)
        {
            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                if (myPositionMarker != null)
                {
                    myPositionMarker.Remove();
                }

                Double lat, lng;
                lat = location.Latitude;
                lng = location.Longitude;

                MarkerOptions markerOpt = new MarkerOptions();
                markerOpt.SetPosition(new LatLng(lat, lng));
                markerOpt.SetTitle("My Location");
                markerOpt.SetSnippet(lat + " : " + lng);
                myPositionMarker = mMap.AddMarker(markerOpt);
                lngLatText.Text = lat + " : " + lng;

                locMgr.RemoveUpdates(this);
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
                            //Permission denied, throw some kind of error here
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
                System.Diagnostics.Debug.WriteLine("No Location");
            else
            {
                //Overlays = GameModel.LoadProximalCells(new LatLng(lastKnownLocation.Latitude, lastKnownLocation.Longitude)); // This loads cells based on player location, NOT camera location.. is that what we want?
            }
        }

        private void CenterMapCameraOnLocation()
        {
            if (myPositionMarker != null)
            {
                CameraUpdate mapCameraPos = CameraUpdateFactory.NewLatLngZoom(myPositionMarker.Position, 10);
                mMap.MoveCamera(mapCameraPos);
            }
        }

        private void LocationButton_Click(object sender, EventArgs e)
        {
            CenterMapCameraOnLocation();
        }

        private void TagButton_Click(object sender, EventArgs e)
        {
            decimal decLat = (decimal)(myPositionMarker.Position.Latitude);
            decimal decLng = (decimal)(myPositionMarker.Position.Longitude);
            int playerCellID = GameModel.GetCellID(decLat, decLng);
            Cell cell = GameModel.CellsInView[playerCellID];

            // Draw the tagged cell on the map
            cell.TeamID = GameModel.Player.Team.ID;
            UpdateOverlay(cell);
            DrawOverlays();

            try
            {
                // Let others know you've tagged this cell
                cell.Tag();
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

        public void UpdateOverlay(Cell updatedCell)
        {
            MapOverlay overlay = new MapOverlay(updatedCell);
            Overlays[updatedCell.ID].SetColor(ColorCode.TeamColor(updatedCell.TeamID));

            DrawOverlays();
        }

        private void DrawOverlays()
        {
            RunOnUiThread(() =>
            {
                if (myPositionMarker.Position != null)
                {
                    double lat = myPositionMarker.Position.Latitude;
                    double lng = myPositionMarker.Position.Longitude;
                    int playerCellID = GameModel.GetCellID((decimal)lat, (decimal)lng);
                    LatLng latlng = GameModel.GetLatLng(playerCellID);

                    mMap.Clear();

                    // Add marker
                    MarkerOptions markerOpt = new MarkerOptions();
                    markerOpt.SetPosition(new LatLng(lat, lng));
                    markerOpt.SetTitle("My Location");
                    markerOpt.SetSnippet(lat + " : " + lng);
                    myPositionMarker = mMap.AddMarker(markerOpt);
                    lngLatText.Text = lat + " : " + lng;
                }

                // Add overlays                    
                foreach (MapOverlay overlay in Overlays.Values)
                {
                    mMap.AddPolygon(overlay.overlay);
                }
            });
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

        public void OnCameraMoveStarted(int reason)
        {
            //throw new NotImplementedException();
        }

        public void OnCameraMove()
        {
            //throw new NotImplementedException();
        }


        /* [[Example Code]] 
         * 
         * 
         * 
         * 
         * 
         * 
         ////New Latitude and Longitude
             LatLng newYorkLatLng = new LatLng(40.776408, -73.9);
             ////New Camera Position with a zoom level
             CameraUpdate mapCameraPos = CameraUpdateFactory.NewLatLngZoom(newYorkLatLng, 10);

             ////Panning map view to camera updated position
                    //mMap.MoveCamera(mapCameraPos);

             ////Creating a marker and setting option to the marker
             MarkerOptions markerOpt = new MarkerOptions();
             markerOpt.SetPosition(newYorkLatLng);
             markerOpt.SetTitle("New York");
             markerOpt.SetSnippet("AKA: The Big Apple");
             markerOpt.Draggable(true);

             ////Adding the marker to be displayed on the map
             mMap.AddMarker(markerOpt);
       
             ////Event after marker finishes being dragged
             mMap.MarkerDragEnd += MMap_MarkerDragEnd;



            PolygonOptions OiT = new PolygonOptions();
            OiT.Add(new LatLng(45.322012, -122.7635097)); //first rectangle point
            OiT.Add(new LatLng(45.319, -122.7635097));
            OiT.Add(new LatLng(45.319, -122.7735097));
            OiT.Add(new LatLng(45.322012, -122.7735097)); //automatically connects last two points

            OiT.InvokeFillColor(Color.Argb(120, 255, 105, 180)); //Transparent int [0-255] 255 being opaque
            OiT.InvokeStrokeWidth(0);

            Polygon polygonOit = mMap.AddPolygon(OiT);


            PolygonOptions rectangle2 = new PolygonOptions();
            rectangle2.Add(new LatLng(47.35, -122.0)); //first rectangle point
            rectangle2.Add(new LatLng(47.45, -122.0));
            rectangle2.Add(new LatLng(47.45, -122.2));
            rectangle2.Add(new LatLng(47.35, -122.2)); //automatically connects last two points

            rectangle2.InvokeFillColor(Color.Argb(120, 20, 50, 0)); //Transparent int [0-255] 255 being opaque
            rectangle2.InvokeStrokeWidth(0);
            
            Polygon polygon = mMap.AddPolygon(rectangle2);
         
      
         
         */
    }
}