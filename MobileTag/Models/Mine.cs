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
    class Mine
    {
        public int MineID { get; }
        public int CellID { get; }
        public int PlayerID { get; }

        public Mine(int mineID, int cellID, int playerID)
        {
            MineID = mineID;
            CellID = cellID;
            PlayerID = playerID;
        }
    }
}