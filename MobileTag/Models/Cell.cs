using System;
using MobileTag.SharedCode;
using Newtonsoft.Json;

namespace MobileTag.Models
{
    public class Cell
    {
        public int ID { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }

        public int TeamID { get; set; }

        //public int HoldStrength { get { return Database.GetCellHoldStrength(ID); } }

        public bool AreEqual(Cell obj1, Cell obj2)
        {
            return (obj1.Latitude == obj2.Latitude && obj1.Longitude == obj2.Longitude);
        }

        //CTOR's
        public Cell(decimal lat, decimal lng)
        {
            ID = GameModel.GetCellID(lat, lng);
            Latitude = lat;
            Longitude = lng;
            TeamID = 0;
        }
      
        public Cell(int id, decimal lat, decimal lng, int teamID)
        {
            ID = id;
            Latitude = lat;
            Longitude = lng;
            TeamID = teamID;
        }

        [JsonConstructor]
        public Cell(int id)
        {
            ID = id;
            Latitude = 0;
            Longitude = 0;
            TeamID = 0;
        }

        public void Tag()
        {
            TeamID = GameModel.Player.Team.ID;
            
            BroadcastCellUpdate();
            Database.UpdateCell(this, TeamID);
        }

        // Broadcast the updated cell to all of the clients that are currently looking at this cell
        async private void BroadcastCellUpdate()
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