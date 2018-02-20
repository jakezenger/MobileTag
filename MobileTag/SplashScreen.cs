using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    [Activity(Label = "SplashScreen", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class SplashScreen : AppCompatActivity
    {
        private GifImageView gifImageView;
        //private ProgressBar progressBar;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
            gifImageView = FindViewById<GifImageView>(Resource.Id.gifImageView);
            //progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            Random randomNumber = new Random();
            if(randomNumber.Next() % 2 == 0)
            {
                Stream input = Assets.Open("slidin_squares.gif");
                byte[] bytes = ConvertFileToByteArray(input);
                gifImageView.SetBytes(bytes);
                gifImageView.StartAnimation();
            }
            else
            {
                Stream input = Assets.Open("boil.gif");
                byte[] bytes = ConvertFileToByteArray(input);
                gifImageView.SetBytes(bytes);
                gifImageView.StartAnimation();
            }

            //Wait for 6 seconds and start new Activity
            Timer timer = new Timer();
            timer.Interval = 6000;
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Toast.MakeText(this, "Logging in...", ToastLength.Long).Show();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            StartActivity(new Intent(this, typeof(LoginActivity)));
        }

        private byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while((read = input.Read(buffer,0,buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
    //Reference: https://www.youtube.com/watch?v=Ft7AwZLRo4Y
}