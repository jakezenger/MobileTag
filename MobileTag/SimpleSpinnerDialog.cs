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
    [Activity(Label = "SimpleSpinnerDialog")]
    public class SimpleSpinnerDialog : DialogFragment
    {
        public string Message { get; set; }
        public string[] Options { get; set; }
        public EventHandler<long> PositiveHandler { get; set; }
        Spinner spinner;
        private View mView;
        private bool validItemSelected;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            // Populate spinner choices
            spinner = mView.FindViewById<Spinner>(Resource.Id.changeTeamSpinner);
            ArrayAdapter adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, Options);
            spinner.Adapter = adapter;

            spinner.ItemSelected += SelectItem_ItemSelected;

            return mView;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            mView = Activity.LayoutInflater.Inflate(Resource.Layout.SimpleSpinnerDialog, null);

            builder.SetView(mView);
            builder.SetMessage(Message);
            builder.SetPositiveButton("Save", (e, o) =>
            {
                if (validItemSelected)
                    PositiveHandler(e, spinner.SelectedItemId);
            });
            builder.SetNeutralButton("Cancel", (e, o) => { });
            builder.SetCancelable(true);

            return builder.Create();
        }

        // Validates spinner
        private void SelectItem_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Id == 0)
            {
                // User has selected the spinner prompt, not a valid selection
                validItemSelected = false;
            }
            else
            {
                validItemSelected = true;
            }
        }

        public SimpleSpinnerDialog(string message, string[] options, EventHandler<long> handler = null)
        {
            Message = message;
            Options = options;
            PositiveHandler = handler;
        }
    }
}