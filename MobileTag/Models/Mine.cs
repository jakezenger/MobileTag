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
        public int Bucket { get; set; }

        public Mine(int cellID, int playerID, int bucket = 0)
        {
            CellID = cellID;
            PlayerID = playerID;
            Bucket = bucket;
        }

        public int Yield()
        {
            int bucketTemp = Bucket;
            Bucket = 0;

            return bucketTemp;
        }
    }
}