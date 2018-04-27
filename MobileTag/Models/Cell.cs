using System;
using System.Threading.Tasks;
using Android.Gms.Maps.Model;
using MobileTag.SharedCode;
using Newtonsoft.Json;

namespace MobileTag.Models
{
    public class Cell
    {
        private int teamID = 0;
        private int holdStrength = 0;

        public int ID { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public int TeamID
        {
            get
            {
                return teamID;
            }
            set
            {
                teamID = value;

                if (MapOverlay != null)
                    MapOverlay.UpdateColor(holdStrength, teamID);
            }
        }

        public MapOverlay MapOverlay { get; set; }

        public int HoldStrength
        {
            get
            {
                return holdStrength;
            }
            set
            {
                if (value <= GameModel.maxHoldStrength)
                    holdStrength = value;
                else
                    holdStrength = GameModel.maxHoldStrength;

                if (MapOverlay != null)
                    MapOverlay.UpdateColor(holdStrength, teamID);
            }
        }

        public bool AreEqual(Cell obj1, Cell obj2)
        {
            return (obj1.Latitude == obj2.Latitude && obj1.Longitude == obj2.Longitude);
        }

        //CTOR's
        public Cell(decimal lat, decimal lng)
        {
            ID = FindID(lat, lng);
            Latitude = Math.Floor(lat / GameModel.frontierInterval) * GameModel.frontierInterval;
            Longitude = Math.Floor(lng / GameModel.frontierInterval) * GameModel.frontierInterval;
            TeamID = 0;
            HoldStrength = 0;
            MapOverlay = new MapOverlay(this);
        }

        [JsonConstructor]
        public Cell(int id, decimal latitude, decimal longitude, int teamID, int holdStrength = 0)
        {
            ID = id;
            Latitude = latitude;
            Longitude = longitude;
            TeamID = teamID;
            HoldStrength = holdStrength;
            MapOverlay = new MapOverlay(this);
        }

        public Cell(int id)
        {
            ID = id;
            Latitude = GameModel.frontierLowerLeftLat + (id / GameModel.GridWidth * GameModel.frontierInterval);
            Longitude = GameModel.frontierLowerLeftLong + (id % GameModel.GridWidth * GameModel.frontierInterval);
            TeamID = 0;
            HoldStrength = 0;
            MapOverlay = new MapOverlay(this);
        }

        public static int FindID(decimal lat, decimal lng)
        {
            decimal nearestLatInterval, nearestLongInterval;

            nearestLatInterval = (Math.Floor((lat - GameModel.frontierLowerLeftLat) / GameModel.frontierInterval) * GameModel.frontierInterval);
            nearestLongInterval = (Math.Floor((lng - GameModel.frontierLowerLeftLong) / GameModel.frontierInterval) * GameModel.frontierInterval);

            int id = 0;

            if (IsInFrontier(lat, lng))
            {
                id = Convert.ToInt32((nearestLongInterval / GameModel.frontierInterval) + (nearestLatInterval / GameModel.frontierInterval)
                    * GameModel.GridWidth);
            }

            return id;
        }

        public static bool IsInFrontier(decimal lat, decimal lng)
        {
            return (lat >= GameModel.frontierLowerLeftLat && lat <= GameModel.frontierUpperRightLat && lng >= GameModel.frontierLowerLeftLong && lng <= GameModel.frontierUpperRightLong);
        }

        public static bool IsInFrontier(LatLng latLng)
        {
            return IsInFrontier((decimal)latLng.Latitude, (decimal)latLng.Longitude);
        }

        public static int FindID(LatLng latLng)
        {
            return FindID((decimal)latLng.Latitude, (decimal)latLng.Longitude);
        }

        public static LatLng FindLatLng(int cellID)
        {
            decimal lat = GameModel.frontierLowerLeftLat + (cellID / GameModel.GridWidth * GameModel.frontierInterval);
            decimal lng = GameModel.frontierLowerLeftLong + (cellID % GameModel.GridWidth * GameModel.frontierInterval);

            LatLng latLng = new LatLng((double)lat, (double)lng);

            return latLng;
        }

        public async Task Tag()
        {
            if (TeamID == GameModel.Player.Team.ID)
            {
                HoldStrength += 100;
            }
            else 
            {
                if (HoldStrength >= 100)
                {
                    HoldStrength -= 100;
                }
                else
                {
                    TeamID = GameModel.Player.Team.ID;

                    await GameModel.Player.Wallet.AddConfinium(GameModel.DEFAULT_TAG_AMOUNT);

                    HoldStrength = 100;
                }
            }

            await Database.UpdateCell(this);
            await BroadcastCellUpdate();
        }

        // Broadcast the updated cell to all of the clients that are currently looking at this cell
        async private Task BroadcastCellUpdate()
        {
            if (CellHub.Connection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                try
                {
                    await CellHub.HubProxy.Invoke("UpdateCell", this);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}