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
    class GameModel : Application
    {
        //public List<Cell> CellsInView { get; set; } -- SHOULD MAYBE GO IN MAP ACTIVITY... DOES THIS NEED TO BE HERE?
        public Player Player { get; set; }
    }
}