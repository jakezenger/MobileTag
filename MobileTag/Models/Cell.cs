﻿using System;
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
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace MobileTag.Models
{
    public class Cell
    {
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int TeamID
        {
            get { return Database.GetCellTeam(ID); }
            set { }
        }

        //public int HoldStrength { get { return Database.GetCellHoldStrength(ID); } }

        public bool AreEqual(Cell obj1, Cell obj2)
        {
            return (obj1.Latitude == obj2.Latitude && obj1.Longitude == obj2.Longitude);
        }

        //CTOR's
        public Cell(double lat, double lng)
        {
            ID = GameModel.GetCellID(lat, lng);
            Latitude = lat;
            Longitude = lng;
        }
      
        public Cell(int id, double lat, double lng)
        {
            ID = id;
            Latitude = lat;
            Longitude = lng;
        }

        [JsonConstructor]
        public Cell(int id)
        {
            ID = id;
            Latitude = 0;
            Longitude = 0;
        }
    }
}