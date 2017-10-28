using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace MobileTag
{
    [Activity(Label = "MobileTag", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // FOR TESTING PURPOSES ONLY:
            Button goToCreateAccountActivity = FindViewById<Button>(Resource.Id.testCreateAccountActivity);
            goToCreateAccountActivity.Click += GoToCreateAccountActivity_Click;
        }

        private void GoToCreateAccountActivity_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(CreateAccountActivity));
            StartActivity(intent);
        }
    }
}
