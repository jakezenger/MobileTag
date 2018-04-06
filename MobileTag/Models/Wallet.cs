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
    public class Wallet
    {
        public int Confinium { get; set; }

        public void AddConfinium(int amountToAdd)
        {
            Confinium = Confinium + amountToAdd;
        }

        public bool SubtractConfinium(int amountToSubtract)
        {
            if((Confinium - amountToSubtract) > 0)
            {
                Confinium = Confinium - amountToSubtract;
                return true;
            }
            return false;
        }
    }
}