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

            TextView teamNameTextView = FindViewById<TextView>(Resource.Id.teamNameTextView);
            teamNameTextView.Text = GameModel.Player.Team.TeamName;

            TextView cellsClaimedTextView = FindViewById<TextView>(Resource.Id.cellsClaimedLabelTextView);

            cellsClaimedTextView.Text = cellsClaimedTextView.Text + " To be Added";

            ImageView myView = FindViewById<ImageView>(Resource.Id.profilePicImageView);

            myView.Click += MyView_Click;
        }

        private void MyView_Click(object sender, EventArgs e)
        {
            Toast myToast = Toast.MakeText(this, "Clicked the Image", ToastLength.Long);
            myToast.Show();
        }
    }
}