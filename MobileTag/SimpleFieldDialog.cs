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
    [Activity(Label = "SimpleFieldDialog")]
    public class SimpleFieldDialog : DialogFragment
    {
        public string Message { get; set; }
        public string Hint { get; set; }
        public EventHandler<string> PositiveHandler { get; set; }
        private EditText field;
        private View mView;
        private Android.Text.InputTypes inputType = Android.Text.InputTypes.TextVariationPersonName;
        public Android.Text.InputTypes InputType {
            get { return inputType; }
            set {
                inputType = value;

                if (field != null)
                    field.InputType = Android.Text.InputTypes.ClassText | inputType;
            }
        }
        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            field = mView.FindViewById<EditText>(Resource.Id.simpleField);
            field.Hint = Hint;
            field.InputType = Android.Text.InputTypes.ClassText | InputType;

            return mView;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            mView = Activity.LayoutInflater.Inflate(Resource.Layout.SimpleFieldDialog, null);

            builder.SetView(mView);
            builder.SetMessage(Message);
            builder.SetPositiveButton("Save", (e, o) => PositiveHandler(e, field.Text));
            builder.SetNeutralButton("Cancel", (e, o) => { });
            builder.SetCancelable(true);

            return builder.Create();
        }

        public SimpleFieldDialog(string message, string hint, EventHandler<string> handler = null)
        {
            Message = message;
            Hint = hint;
            PositiveHandler = handler;
        }
    }
}