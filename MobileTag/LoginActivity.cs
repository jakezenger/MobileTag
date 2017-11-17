﻿using System;
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
    [Activity(Label = "LoginActivity", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private bool validUsername = false;
        private bool usernameChanged = false;
        private bool passwordChanged = false;
        private bool validPassword = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            // Set event handlers
            Button signInButton = FindViewById<Button>(Resource.Id.signInButton);
            signInButton.Click += SignInButton_Click;
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);
            usernameField.FocusChange += UsernameField_FocusChange;
            usernameField.TextChanged += UsernameField_TextChanged;
            passwordField.FocusChange += PasswordField_FocusChange;
            passwordField.TextChanged += PasswordField_TextChanged;
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);

            if (Database.ValidateLoginCredentials(usernameField.Text, passwordField.Text) == 1)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);

                builder.SetCancelable(false);
                builder.SetPositiveButton(GetString(Resource.String.ok), (s, ea) => { /* User has clicked ok */ });
                builder.SetTitle(GetString(Resource.String.login_failed));
                builder.SetMessage(GetString(Resource.String.correct_credentials_prompt));

                AlertDialog loginFailedAlert = builder.Create();

                loginFailedAlert.Show();
            }
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

        // Check to see if user has entered valid account data... if they have, enable signInButton
        private void Validate()
        {
            Button signInButton = FindViewById<Button>(Resource.Id.signInButton);
            EditText usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            EditText passwordField = FindViewById<EditText>(Resource.Id.passwordField);
            Spinner selectTeam = FindViewById<Spinner>(Resource.Id.selectTeamSpinner);

            if (!validUsername && usernameChanged)
            {
                usernameField.Error = GetString(Resource.String.invalid_username_prompt);
            }
            else
            {
                usernameField.Error = null;
            }

            if (!validPassword && passwordChanged)
            {
                passwordField.Error = GetString(Resource.String.invalid_password_prompt);
            }
            else
            {
                passwordField.Error = null;
            }

            if (validUsername && validPassword)
            {
                signInButton.Enabled = true;
            }
            else
            {
                signInButton.Enabled = false;
            }
        }
    }
}