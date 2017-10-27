using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileTag
{
    [Activity(Label = "CreateAccountActivity", MainLauncher = true)]
    public class CreateAccountActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateAccount);

            string[] teams = { GetString(Resource.String.select_team_prompt), "Red", "Green", "Blue", "Purple", "Pink" };

            // Populate selectTeamSpinner choices
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, teams);
            selectTeam.Adapter = adapter;
        }
    }
}