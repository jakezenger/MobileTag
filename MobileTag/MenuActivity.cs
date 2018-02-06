using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Android.Views;

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

            

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "MobileTag";

            FindViews();
            HandleEvents();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted, 
            ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
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

