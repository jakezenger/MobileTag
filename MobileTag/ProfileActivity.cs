using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using MobileTag.Models;

using File = System.IO.File;
using Path = System.IO.Path;
using String = System.String;


namespace MobileTag
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class ProfileActivity : Activity
    {
        //stuff for images https://github.com/xamarin/recipes/tree/master/Recipes/android/other_ux/pick_image
        private ImageView myView;
        public static readonly int PickImageId = 1000;
        private byte[] pictByteArray;
        //drawer stuff
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Random randomNumber = new Random();
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Profile);
            
            SetUpUI();

            TextView usernameTextView = FindViewById<TextView>(Resource.Id.usernameTextView);
            usernameTextView.Text = GameModel.Player.Username;

            TextView teamNameTextView = FindViewById<TextView>(Resource.Id.teamNameTextView);
            teamNameTextView.Text = GameModel.Player.Team.TeamName;

            TextView cellsClaimedTextView = FindViewById<TextView>(Resource.Id.cellsClaimedLabelTextView);

            cellsClaimedTextView.Text = cellsClaimedTextView.Text + randomNumber.Next();

           myView = FindViewById<ImageView>(Resource.Id.profilePicImageView);

            TextView confiniumTextView = FindViewById<TextView>(Resource.Id.confiniumTextView);
            confiniumTextView.Text = "c " + GameModel.Player.Wallet.Confinium + "\nMines: " + " number of mines" +
                                     "\n Anti-Mines: " + "number of antimines";
            myView.Click += MyView_Click;
            

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {

                    case Resource.Id.nav_profile:

                        break;
                    case Resource.Id.nav_map:
                        StartActivity(new Intent(this, typeof(MapActivity)));

                        break;
                    case Resource.Id.nav_settings:
                        StartActivity(new Intent(this, typeof(SettingsActivity)));
                        break;
                    case Resource.Id.nav_logout:
                        GameModel.Logout();
                        StartActivity(new Intent(this, typeof(LoginActivity)));
                        break;
                    default:
                        break;
                }

                drawerLayout.CloseDrawers();
            };
            string path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "imagePath.txt");
            if (System.IO.File.Exists(filePath))
            {
                LoadImage(filePath);
            }
            else
            {
                SetImage(myView);
            }
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
        }

        private void MyView_Click(object sender, EventArgs e)
        {
            //let user pick an image
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                myView.SetImageURI(uri);
                String ImagePath = GetRealPathFromURI(uri);
                StoreImagePath(ImagePath);
            }
        }


        private void LoadImage(String filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.FromFile(new Java.IO.File(filePath));

            System.IO.Stream input = this.ContentResolver.OpenInputStream(uri);

            //Use bitarray to use less memory                    
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                pictByteArray = ms.ToArray();
            }

            input.Close();

            //Get file information
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            Bitmap bitmap = BitmapFactory.DecodeByteArray(pictByteArray, 0, pictByteArray.Length, options);

            myView.SetImageBitmap(bitmap);
        }



        private void SetImage(ImageView myView)
        {
            //https://theconfuzedsourcecode.wordpress.com/2016/02/24/load-image-resources-by-name-in-android-xamarin/  that is the site for how i got the image resource id
            // clip art came from https://free.clipartof.com I then used Irfanview to resize and reduce the image.
            int resourceId = (int) typeof(Resource.Drawable).GetField(GameModel.Player.Team.TeamName).GetValue(null);
            myView.SetImageResource(resourceId);
        }

        private void SetImage(ImageView myView, String image)
        {
            int resourceId = (int) typeof(Resource.Drawable).GetField(image).GetValue(null);
            myView.SetImageResource(resourceId);
        }
        
        private void SaveImage(String ImageName, byte[] image)
        {
            var path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filename = Path.Combine(path.ToString(), ImageName + ".Jpeg");
            File.WriteAllBytes(filename, image);
        }
        private string GetRealPathFromURI(Android.Net.Uri contentURI)
        {
            string path = null;
            // The projection contains the columns we want to return in our query.
            string[] projection = new[] { Android.Provider.MediaStore.Audio.Media.InterfaceConsts.Data };
            using (ICursor cursor = ManagedQuery(contentURI, projection, null, null, null))
            {
                if (cursor != null)
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Audio.Media.InterfaceConsts.Data);

                    cursor.MoveToFirst();

                    path = cursor.GetString(columnIndex);
                }
            }

            return path;
        }

        private void StoreImagePath(String ImagePath)
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "imagePath.txt");
            System.IO.File.WriteAllText(filePath, ImagePath);
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