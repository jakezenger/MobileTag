﻿using System;
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
    public class AntiMine : Mine
    {
        public AntiMine(int cellID, int playerID) : base(cellID, playerID)
        {

        }
    }
}