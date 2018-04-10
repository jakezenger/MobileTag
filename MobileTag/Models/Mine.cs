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
    public class Mine
    {
        public int CellID { get; }
        public int PlayerID { get; }

        public Mine(int cellID, int playerID)
        {
            CellID = cellID;
            PlayerID = playerID;
        }

        public void Construct()
        {
            throw new NotImplementedException();
            // Add mine to DB... and update UI?
        }
    }
}