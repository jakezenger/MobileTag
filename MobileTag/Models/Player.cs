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
    public class Player
    {
        public string username { get; set; }
        public string password { get; set; }
        public int teamId { get; set; }
    }
}