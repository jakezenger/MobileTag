using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileTag.Models
{
    public class Cell
    {
        public int ID { get; set; }
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        //public Latlng { get; }
        public int TeamID
        {
            get { return Database.GetCellTeam(ID); }
        }
        //public int HoldStrength { get { return Database.GetCellHoldStrength(ID); } }

        //public Cell(Latlng location)
        //{
        //    // GENERATE ID FOR GIVEN LOCATION
        //}
   

        public bool AreEqual(Cell obj1, Cell obj2)
        {
            return (obj1 == obj2);
        }
     
        

        //CTOR's
        public Cell(decimal lat, decimal lng)
        {
            ID = GameModel.GetCellID(lat, lng);
            Latitude = lat;
            Longitude = lng;
        }
      
        public Cell(int id, decimal lat, decimal lng)
        {
            ID = id;
            Latitude = lat;
            Longitude = lng;
        }
        public Cell(int id)
        {
            ID = id;
            Latitude = 0m;
            Longitude = 0m;
        }
    }
}