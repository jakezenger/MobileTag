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
using Android.Graphics;

namespace MobileTag.Models
{
    class Team
    {
        public int ID { get; }
        public string TeamName { get; }
        public Color TeamColor { get; }

        public Team(int id, string teamName, Color teamColor)
        {
            ID = id;
            TeamName = teamName;
            TeamColor = teamColor;
        }
    }
}