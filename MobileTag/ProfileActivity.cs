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
using MobileTag.Models;

namespace MobileTag
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Profile);

            TextView usernameTextView = FindViewById<TextView>(Resource.Id.usernameTextView);
            usernameTextView.Text = GameModel.Player.Username;
            usernameTextView.TextSize = 20.0f;
        }
    }
}