using System;
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
using Uri = Android.Net.Uri;


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
            // Documents folder
            string documentsPath = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal);
           
            var filePath = System.IO.Path.Combine(documentsPath, "imagePath.png");
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
                StoreImage(uri);
            }
        }


        private void LoadImage(String filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.FromFile(new Java.IO.File(filePath));
            System.IO.Stream input = this.ContentResolver.OpenInputStream(uri);
            Android.Graphics.Bitmap mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, uri);
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            Bitmap rotated = Bitmap.CreateBitmap(mBitmap, 0, 0, mBitmap.Width, mBitmap.Height,
                matrix, true);
            myView.SetImageBitmap(rotated);
        }



        private void SetImage(ImageView myView)
        {
            //https://theconfuzedsourcecode.wordpress.com/2016/02/24/load-image-resources-by-name-in-android-xamarin/  that is the site for how i got the image resource id
            // clip art came from https://free.clipartof.com I then used Irfanview to resize and reduce the image.
            int resourceId = (int) typeof(Resource.Drawable).GetField(GameModel.Player.Team.TeamName).GetValue(null);
            myView.SetImageResource(resourceId);
        }

       
        
   
     

        private void StoreImage(Uri incommingURI)
        {
            // Documents folder
            string documentsPath = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal);
           
            var filePath = System.IO.Path.Combine(documentsPath, "imagePath.png");
            System.IO.Stream input = this.ContentResolver.OpenInputStream(incommingURI);
           
            byte[] buffer = new byte[16 * 1024];

            using
            
                //need to convert to bytes to store image
                (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    pictByteArray = ms.ToArray();
                }
           

            
            System.IO.File.WriteAllBytes(filePath, pictByteArray);
            LoadImage(filePath);
        }
       
        private Android.Graphics.Bitmap NGetBitmap(Android.Net.Uri uriImage)
        {
            Android.Graphics.Bitmap mBitmap = null;
            mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, uriImage);
            return mBitmap;
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