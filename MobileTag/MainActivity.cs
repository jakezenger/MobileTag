using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;

namespace MobileTag
{
    [Activity(Label = "MobileTag")]
    public class MainActivity : Activity
    {
        Button mButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mButton = FindViewById<Button>(Resource.Id.btnToMapTest);
            mButton.Click += mButton_Click;
        }

        void mButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MapActivity));
            this.StartActivity(intent);

        }
    }
}

