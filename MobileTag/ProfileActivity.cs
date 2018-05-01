using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProfileActivity : Activity
    {
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Profile);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            toolbar.SetBackgroundColor(ColorCode.TeamColor(GameModel.Player.Team.ID));
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            ActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_dehaze_white_24dp);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout.DrawerStateChanged += DrawerLayout_DrawerStateChanged;

            TextView usernameTextView = FindViewById<TextView>(Resource.Id.usernameTextView);
            usernameTextView.Text = GameModel.Player.Username;

            TextView teamNameTextView = FindViewById<TextView>(Resource.Id.teamNameTextView);
            teamNameTextView.Text = GameModel.Player.Team.TeamName;

            TextView cellsClaimedTextView = FindViewById<TextView>(Resource.Id.cellsClaimedLabelTextView);

            cellsClaimedTextView.Text = cellsClaimedTextView.Text + " To be Added";

            ImageView myView = FindViewById<ImageView>(Resource.Id.profilePicImageView);

            TextView confiniumTextView = FindViewById<TextView>(Resource.Id.confiniumTextView);
            confiniumTextView.Text =  "c " + GameModel.Player.Wallet.Confinium;
            myView.Click += MyView_Click;
            SetImage(myView);

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_logout:
                        GameModel.Logout();
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    case Resource.Id.nav_settings:
                        StartActivity(new Intent(this, typeof(SettingsActivity)));
                        break;
                    case Resource.Id.nav_profile:
                        StartActivity(new Intent(this, typeof(ProfileActivity)));
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawers();
            };

        }

        private void MyView_Click(object sender, EventArgs e)
        {
            Toast myToast = Toast.MakeText(this, "Clicked the Image", ToastLength.Long);
            myToast.Show();
        }

        private void SetImage(ImageView myView)
        {
            //https://theconfuzedsourcecode.wordpress.com/2016/02/24/load-image-resources-by-name-in-android-xamarin/  that is the site for how i got the image resource id
            // clip art came from https://free.clipartof.com I then used Irfanview to resize and reduce the image.
            int resourceId = (int)typeof(Resource.Drawable).GetField(GameModel.Player.Team.TeamName).GetValue(null);
            myView.SetImageResource(resourceId);
        }

        private void SetImage(ImageView myView, String image)
        {
            int resourceId = (int)typeof(Resource.Drawable).GetField(image).GetValue(null);
            myView.SetImageResource(resourceId);
        }

        private void SaveImage(String ImageName, byte[] image)
        {
            var path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = Path.Combine(path.ToString(), ImageName);
            File.WriteAllBytes(filename, image);
        }
        private void DrawerLayout_DrawerStateChanged(object sender, DrawerLayout.DrawerStateChangedEventArgs e)
        {
            TextView usernameHeader = FindViewById<TextView>(Resource.Id.nameTxt);
            usernameHeader.Text = GameModel.Player.Username;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);

            return base.OnOptionsItemSelected(item);
        }
    }
}