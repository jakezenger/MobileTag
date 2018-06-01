using System;
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
        private string[] teams, themes;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            SetUpUI();

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
                        this.FinishAffinity();
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawers();

            };

            teams = new string[] { GetString(Resource.String.select_team_prompt), "Red", "Green", "Blue", "Orange", "Pink" };
            themes = new string[] { "Select theme", "Dark", "Light" };
        }

        public void SetUpUI()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            toolbar.SetBackgroundColor(ColorCode.TeamColor(GameModel.Player.Team.ID));

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            ActionBar.SetHomeAsUpIndicator(Resource.Mipmap.ic_dehaze_white_24dp);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            drawerLayout.DrawerStateChanged += DrawerLayout_DrawerStateChanged;

            Button changeUsernameBtn = FindViewById<Button>(Resource.Id.changeUsernameButton);
            Button changePasswordBtn = FindViewById<Button>(Resource.Id.changePasswordButton);
            Button switchTeamBtn = FindViewById<Button>(Resource.Id.switchTeamButton);
            Button switchMapBtn = FindViewById<Button>(Resource.Id.switchMapStyleButton);

            changeUsernameBtn.Click += ChangeUsernameBtn_Click;
            changePasswordBtn.Click += ChangePasswordBtn_Click;
            switchTeamBtn.Click += SwitchTeamBtn_Click;
            switchMapBtn.Click += SwitchMapBtn_Click;
        }

        private void SwitchMapBtn_Click(object sender, EventArgs e)
        {
            SimpleSpinnerDialog simpleSpinnerDialog = new SimpleSpinnerDialog("Select map theme:", themes);
            simpleSpinnerDialog.PositiveHandler += ChangeTheme;
            simpleSpinnerDialog.Show(FragmentManager, "SimpleSpinnerDialog");
        }

        private void ChangeTheme(object sender, long e)
        {
            switch (themes[e])
            {
                case "Dark":
                    GameModel.MapStyle = Resource.Raw.style_json;
                    break;
                case "Light":
                    GameModel.MapStyle = Resource.Raw.StandardTheme;
                    break;
                default:
                    GameModel.MapStyle = Resource.Raw.style_json;
                    break;
            }

            Toast.MakeText(this, "Set map theme to " + themes[e] + ".", ToastLength.Short).Show();
        }

        private void SwitchTeamBtn_Click(object sender, EventArgs e)
        {
            SimpleSpinnerDialog simpleSpinnerDialog = new SimpleSpinnerDialog("Select new team:", teams);
            simpleSpinnerDialog.PositiveHandler += ChangeTeam;
            simpleSpinnerDialog.Show(FragmentManager, "SimpleSpinnerDialog");
        }

        private void ChangeTeam(object sender, long e)
        {
            int teamID = Array.IndexOf(teams, teams[e]) - 1;

            Toast.MakeText(this, "Switched to the " + teams[e] + " faction.", ToastLength.Short).Show();
        }

        private void ChangePasswordBtn_Click(object sender, EventArgs e)
        {
            SimpleFieldDialog simpleFieldDialog = new SimpleFieldDialog("Enter new password:", "new password");
            simpleFieldDialog.PositiveHandler += ChangePassword;
            simpleFieldDialog.Show(FragmentManager, "SimpleFieldDialog");
        }

        private void ChangePassword(object sender, string e)
        {
            Toast.MakeText(this, "Password has been changed.", ToastLength.Short).Show();
        }

        private void ChangeUsernameBtn_Click(object sender, EventArgs e)
        {
            SimpleFieldDialog simpleFieldDialog = new SimpleFieldDialog("Enter new username:", "new username");
            simpleFieldDialog.PositiveHandler += ChangeUsername;
            simpleFieldDialog.Show(FragmentManager, "SimpleFieldDialog");
        }

        private void ChangeUsername(object sender, string e)
        {
            Toast.MakeText(this, "Username has been changed.", ToastLength.Short).Show();
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
            Toast.MakeText(this, "Map style updated.", ToastLength.Long).Show();

            this.Finish();
            var intent = new Intent(this, typeof(MapActivity)).SetFlags(ActivityFlags.ClearTask);
            StartActivity(intent);
        }
      };
}
