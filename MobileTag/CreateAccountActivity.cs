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
        private bool validTeamSelected = false;
        private bool validUsername = false;
        private bool usernameChanged = false;
        private bool passwordChanged = false;
        private bool validPassword = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateAccount);

            string[] teams = { GetString(Resource.String.select_team_prompt), "Red", "Green", "Blue", "Purple", "Pink" };

            // Populate selectTeamSpinner choices
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, teams);
            selectTeam.Adapter = adapter;

            // Set event handlers on username, password, and team spinner for data validation
            selectTeam.ItemSelected += SelectTeam_ItemSelected;
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);
            usernameField.FocusChange += UsernameField_FocusChange;
            usernameField.TextChanged += UsernameField_TextChanged;
            passwordField.FocusChange += PasswordField_FocusChange;
            passwordField.TextChanged += PasswordField_TextChanged;

            // Set event handler for "Done" button
            Button doneButton = FindViewById<Button>(Resource.Id.createAccountButton);
            doneButton.Click += DoneButton_Click;
        }

        private void PasswordField_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!passwordChanged)
                passwordChanged = true;

            EditText field = (EditText)sender;

            if (field.Text == "")
                validPassword = false;
            else
                validPassword = true;

            Validate();
        }

        private void PasswordField_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            Validate();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);
            Spinner teamSpinner = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);

            int teamID = (int)teamSpinner.SelectedItemId;
            string username = usernameField.Text;
            string password = passwordField.Text;

            //SQLtest.TestInsert(username, password, teamID);
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
            if (e.Id == 0)
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
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);

            if (!validUsername && usernameChanged)
            {
                usernameField.Error = "Please enter a valid username.";
            }
            else
            {
                usernameField.Error = null;
            }

            if (!validPassword && passwordChanged)
            {
                passwordField.Error = "Please enter a valid password.";
            }
            else
            {
                passwordField.Error = null;
            }

            if (validTeamSelected && validUsername && validPassword)
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