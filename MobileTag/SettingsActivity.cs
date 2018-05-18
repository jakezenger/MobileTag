using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MobileTag.Models;

namespace MobileTag
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class SettingsActivity : Activity
    {
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            SetUpUI();
            CheckBox checkbox = FindViewById<CheckBox>(Resource.Id.CheckBox_MapStyleLight);
            Button SaveButton = FindViewById<Button>(Resource.Id.Save_Button);
            SaveButton.Click += SaveButton_Click;
            // todo: make this checkbox do something other than display it was selected
            if (GameModel.MapStyle == Resource.Raw.StandardTheme)
                checkbox.Checked = true;
            else
                checkbox.Checked = false;

            //Drawer navigation menu event handler
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_profile:
                        this.Finish();
                        StartActivity(new Intent(this, typeof(ProfileActivity)));
                        break;
                    case Resource.Id.nav_map:
                        this.Finish();
                        StartActivity(new Intent(this, typeof(MapActivity)));
                        break;
                    case Resource.Id.nav_settings:
                        //we are already in settings activity
                        break;
                    case Resource.Id.nav_logout:
                        GameModel.Logout();
                        this.Finish();
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawers();

            };
        }

        public void SetUpUI()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            toolbar.SetBackgroundColor(ColorCode.TeamColor(GameModel.Player.Team.ID));

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            CheckBox checkbox = FindViewById<CheckBox>(Resource.Id.CheckBox_MapStyleLight);
            Button SaveButton = FindViewById<Button>(Resource.Id.Save_Button);

            ActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_dehaze_white_24dp);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout.DrawerStateChanged += DrawerLayout_DrawerStateChanged;
          
            SaveButton.Click += SaveButton_Click; ;
        }

        private void DrawerLayout_DrawerStateChanged(object sender, DrawerLayout.DrawerStateChangedEventArgs e)
        {
            TextView usernameHeader = FindViewById<TextView>(Resource.Id.nameTxt);
            TextView userConfinium = FindViewById<TextView>(Resource.Id.confiniumTxt);
            usernameHeader.Text = GameModel.Player.Username;
            userConfinium.Text = GameModel.Player.Wallet.Confinium.ToString();
        }

        //Hamburger Button Pressed
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);

            return base.OnOptionsItemSelected(item);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "TestSave", ToastLength.Long).Show();
            CheckBox checkbox = FindViewById<CheckBox>(Resource.Id.CheckBox_MapStyleLight);

            GameModel.MapStyle = checkbox.Checked ? Resource.Raw.StandardTheme : Resource.Raw.style_json;

            this.Finish();
            var intent = new Intent(this, typeof(MapActivity)).SetFlags(ActivityFlags.ClearTask);
            StartActivity(intent);
        }
      };
}
