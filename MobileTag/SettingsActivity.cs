using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MobileTag.Models;

namespace MobileTag
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            CheckBox checkbox = FindViewById<CheckBox>(Resource.Id.CheckBox_MapStyleLight);
            Button SaveButton = FindViewById<Button>(Resource.Id.Save_Button);
            SaveButton.Click += SaveButton_Click;
            // todo: make this checkbox do something other than display it was selected
            if (GameModel.MapStyle == Resource.Raw.StandardTheme)
                checkbox.Checked = true;
            else
                checkbox.Checked = false;
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
