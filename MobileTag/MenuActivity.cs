using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;

namespace MobileTag
{
    [Activity(Label = "MobileTag")]
    public class MenuActivity : Activity
    {
        Button mapButton;
        Button profileButton;
        Button communityButton;
        Button settingsButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Menu);

            //var userID = Intent.Extras.GetUserID("UserID", 0);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            mapButton = FindViewById<Button>(Resource.Id.btnViewMap);
            profileButton = FindViewById<Button>(Resource.Id.btnViewProfile);
            communityButton = FindViewById<Button>(Resource.Id.btnCommunity);
            settingsButton = FindViewById<Button>(Resource.Id.btnSettings);

        }

        private void HandleEvents()
        {
            mapButton.Click += MapButton_Click;
            profileButton.Click += ProfileButton_Click;
            communityButton.Click += CommunityButton_Click;
            settingsButton.Click += SettingsButton_Click;
        }

        private void MapButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(MapActivity));
            StartActivity(intent);

        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(ProfileActivity));
            StartActivity(intent);
        }

        private void CommunityButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CommunityActivity));
            StartActivity(intent);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }
    }
}

