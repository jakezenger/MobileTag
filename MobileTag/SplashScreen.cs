using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Felipecsl.GifImageViewLibrary;
using MobileTag.Models;

namespace MobileTag
{
    [Activity(Label = "@string/app_name", MainLauncher = true, NoHistory = true,  Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class SplashScreen : AppCompatActivity
    {
        private GifImageView gifImageView;
        //private ProgressBar progressBar;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
            gifImageView = FindViewById<GifImageView>(Resource.Id.gifImageView);

            Stream input = Assets.Open("slidin_squares.gif");
            byte[] bytes = ConvertFileToByteArray(input);
            gifImageView.SetBytes(bytes);
            gifImageView.StartAnimation();

            //Wait for 6 seconds and start new Activity
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Toast.MakeText(this, "Logging in...", ToastLength.Long).Show();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await CheckLocalLoginInformation();
            ((System.Timers.Timer)sender).Stop();
        }

        private async Task CheckLocalLoginInformation()
        {
            // Check for local login information
            string path = Application.Context.FilesDir.Path;
            string filePath = System.IO.Path.Combine(path, "username.txt");
            string filePath2 = System.IO.Path.Combine(path, "password.txt");
            string username, password;

            if (System.IO.File.Exists(filePath) && System.IO.File.Exists(filePath2))
            {
                username = System.IO.File.ReadAllText(filePath);
                password = System.IO.File.ReadAllText(filePath2);

                int result = await Database.ValidateLoginCredentials(username, password);

                if (result == 1)
                {
                    GameModel.Player = await Database.GetPlayer(username.Trim());

                    Intent intent = new Intent(this, typeof(MapActivity));

                    StartActivity(intent);
                }
                else

                    StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            else
                StartActivity(new Intent(this, typeof(LoginActivity)));
        }

        private byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
    //Reference: https://www.youtube.com/watch?v=Ft7AwZLRo4Y
}