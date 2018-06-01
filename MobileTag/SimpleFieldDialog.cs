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
        public EventHandler<Android.Text.TextChangedEventArgs> TypingValidationHandler { get; set; }
        public Func<string, bool> FinalValidationHandler;
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

            if (TypingValidationHandler != null)
            {
                field.TextChanged += TypingValidationHandler;
            }

            field.InputType = Android.Text.InputTypes.ClassText | InputType;

            return mView;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);

            mView = Activity.LayoutInflater.Inflate(Resource.Layout.SimpleFieldDialog, null);

            builder.SetView(mView);
            builder.SetMessage(Message);
            builder.SetPositiveButton("Save", (o, e) =>
            {
                PositiveButton_Click(o, e);
            });

            builder.SetNeutralButton("Cancel", (e, o) => { });
            builder.SetCancelable(true);

            AlertDialog alertDialog = builder.Create();

            alertDialog.ShowEvent += (o, e) =>
            {
                Button posButton = alertDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
                posButton.Click += PositiveButton_Click;
            };

            return alertDialog;
        }

        private void PositiveButton_Click(object sender, EventArgs e)
        {
            if (FinalValidationHandler != null)
            {
                try
                {
                    if (FinalValidationHandler(field.Text))
                    {
                        PositiveHandler(e, field.Text);
                        Dismiss();
                    }
                }
                catch (Exception exc)
                {
                    field.Error = exc.Message;
                }
            }
            else
            {
                PositiveHandler(e, field.Text);
                Dismiss();
            }
        }

        public SimpleFieldDialog(string message, string hint)
        {
            Message = message;
            Hint = hint;
        }
    }
}