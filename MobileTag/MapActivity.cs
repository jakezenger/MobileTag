using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileTag.Models;

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
using MobileTag.SharedCode;

namespace MobileTag
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity, IOnMapReadyCallback, Android.Locations.ILocationListener
    {
        private GoogleMap mMap;
        private LocationManager locMgr;
        private String provider;
        private Location lastKnownLocation;
        private Marker myPositionMarker;
        private TextView lngLatText;
        private Button tagButton;
        private Button locationButton;
        readonly string[] LocationPermissions = { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation };
        private const int RequestLocationID = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            ////Connects Map.axml to this Activity
            SetContentView(Resource.Layout.Map);
            lngLatText = FindViewById<TextView>(Resource.Id.textBelowMap);
            tagButton = FindViewById<Button>(Resource.Id.claimButton);
            locationButton = FindViewById<Button>(Resource.Id.clientCameraLocationbtn);
            tagButton.Click += TagButton_Click;
            locationButton.Click += LocationButton_Click;

            SetUpMap();



            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                // Location permissions have been granted
                GetLocation();
            }
            else
            {
                RequestPermissions(LocationPermissions, RequestLocationID);
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
                            // Permission was denied.. go back to the MenuActivity
                            StartActivity(new Intent(this, typeof(MenuActivity)));
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

            //foreach (Cell cell in GameModel.Frontier)
            //{
            //    Color color = ColorCode.SetTeamColor(3);
            //    UpdateOverlay(cell, color);
            //}

            provider = locMgr.GetBestProvider(locationCriteria, true);

            lastKnownLocation = locMgr.GetLastKnownLocation(provider);
            if (lastKnownLocation == null)
                System.Diagnostics.Debug.WriteLine("No Location");
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
            //PolygonOptions squareOverlay = new PolygonOptions();
            //squareOverlay.Add(new LatLng(myPositionMarker.Position.Latitude - .005, myPositionMarker.Position.Longitude + .005)); //first rectangle point
            //squareOverlay.Add(new LatLng(myPositionMarker.Position.Latitude + .005, myPositionMarker.Position.Longitude + .005));
            //squareOverlay.Add(new LatLng(myPositionMarker.Position.Latitude + .005, myPositionMarker.Position.Longitude - .005));
            //squareOverlay.Add(new LatLng(myPositionMarker.Position.Latitude - .005, myPositionMarker.Position.Longitude - .005)); //automatically connects last two points

            //squareOverlay.InvokeFillColor(GameModel.Player.teamColor); //Transparent (alpha) int [0-255] 255 being opaque
            //squareOverlay.InvokeStrokeWidth(0);

            //Polygon polygonOit = mMap.AddPolygon(squareOverlay);

            decimal decLat = 90.0m - (decimal)myPositionMarker.Position.Latitude;
            decimal decLng = 180.0m + (decimal)myPositionMarker.Position.Longitude;
            int playerCellID = GameModel.GetCellID(decLat, decLng);

            if(playerCellID >= 0 && playerCellID < GameModel.Frontier.Capacity)
            {
                GetLocation();
                Database.UpdateCell(playerCellID, GameModel.Player.Team.ID);
                GameModel.Overlays[playerCellID].UpdateColor();
            }

            foreach (MapOverlay overlay in GameModel.Overlays)
            {
                mMap.AddPolygon(overlay.overlay);
            }
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
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                ////This will remove the listener from constantly grabbing locations
                locMgr.RemoveUpdates(this);
            }
        }


        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        /*This function is called from SetUpMap()*/
        public void OnMapReady(GoogleMap googleMap)
        {
            ////Map Creation
            mMap = googleMap;

            ////Event after marker finishes being dragged
            mMap.MarkerDragEnd += MMap_MarkerDragEnd;

        }
        private void MMap_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 10));
            Console.WriteLine(pos.ToString());
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

                //locMgr.RemoveUpdates(this);
                
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

        public void UpdateOverlay(Cell cell, Color color)
        {
            PolygonOptions squareOverlay = new PolygonOptions();
            squareOverlay.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude)); //first rectangle point
            squareOverlay.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            squareOverlay.Add(new LatLng((double)cell.Latitude - (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            squareOverlay.Add(new LatLng((double)cell.Latitude - (double)GameModel.FrontierInterval, (double)cell.Longitude)); //automatically connects last two points

            squareOverlay.InvokeFillColor(color); //Transparent (alpha) int [0-255] 255 being opaque
            squareOverlay.InvokeStrokeWidth(0);

            mMap.AddPolygon(squareOverlay);
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