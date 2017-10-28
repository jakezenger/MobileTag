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
    [Activity(Label = "Create Account")]
    public class CreateAccountActivity : Activity
    {
        bool validTeamSelected = false;
        bool validUsername = false;
        bool usernameChanged = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateAccount);

            string[] teams = { GetString(Resource.String.select_team_prompt), "Red", "Green", "Blue", "Purple", "Pink" };

            // Populate selectTeamSpinner choices
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, teams);
            selectTeam.Adapter = adapter;

            // Set event handlers on username and team spinner for data validation
            selectTeam.ItemSelected += SelectTeam_ItemSelected;
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            usernameField.FocusChange += UsernameField_FocusChange;
            usernameField.TextChanged += UsernameField_TextChanged;
        }

        // Validates usernameField
        private void UsernameField_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!usernameChanged)
                usernameChanged = true;

            EditText field = (EditText)sender;

            if (field.Text == "")
                validUsername = false;
            else
                validUsername = true;

            Validate();
        }

        private void UsernameField_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            Validate();
        }

        // Validates selectTeamSpinner
        private void SelectTeam_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if(e.Id == 0)
            {
                // User has selected the spinner prompt, not a valid team selection
                validTeamSelected = false;
            }
            else
            {
                validTeamSelected = true;
            }

            Validate();
        }

        // Check to see if user has entered valid account data... if they have, enable "DONE" button
        private void Validate()
        {
            Button doneButton = FindViewById<Button>(Resource.Id.createAccountButton);
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);

            if (!validUsername && usernameChanged)
            {
                usernameField.Error = "Please enter a valid username.";
            }
            else
            {
                usernameField.Error = null;
            }

            if (validTeamSelected && validUsername)
            {
                doneButton.Enabled = true;
            }
            else
            {
                doneButton.Enabled = false;
            }
        }
    }
}