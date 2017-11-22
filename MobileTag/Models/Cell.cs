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

namespace MobileTag.Models
{
    public class Cell
    {
        public int ID { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        //public Latlng { get; }
        public int TeamID { get { return Database.GetCellTeam(ID); } }
        //public int HoldStrength { get { return Database.GetCellHoldStrength(ID); } }

        //public Cell(Latlng location)
        //{
        //    // GENERATE ID FOR GIVEN LOCATION
        //}

        //CTOR's
        public Cell(decimal lat, decimal lng)
        {
            ID = GameModel.GetCellID(lat, lng);
            Latitude = lat;
            Longitude = lng;
        }
    }

}