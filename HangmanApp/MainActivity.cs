using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using SQLite;
using Android.Graphics;
using Android.Views;

/*
 * TODO: Create game interface
 * TODO: Create scoring system
 * TODO: Make sure all ui with text to use same font
 */

namespace HangmanApp
{
    [Activity(Label = "HangmanApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView txt_Welcome;
        Button btn_Start;
        Button btn_LogOut;
        Database db;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            db = new Database();
            txt_Welcome = FindViewById<TextView>(Resource.Id.txt_Welcome);
            btn_Start = FindViewById<Button>(Resource.Id.btn_Start);
            btn_LogOut = FindViewById<Button>(Resource.Id.btn_LogOut);

            var font = Typeface.CreateFromAsset(Assets, "iosevka-regular.ttf");
            if (db.IsLoggedIn())
            {
                txt_Welcome.Text = "Welcome " + db.LoggedInAccount(this).Username + " To The HangMan Game";
                btn_LogOut.Visibility = ViewStates.Visible;
            }
            btn_Start.Typeface = font;
            btn_Start.Click += Btn_Start_Click;
            btn_LogOut.Click += Btn_LogOut_Click;
        }

        private void Btn_LogOut_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
            Finish();
        }

        private void Btn_Start_Click(object sender, System.EventArgs e)
        {
            if (db.IsLoggedIn())
            {
                StartActivity(typeof(GameActivity));
                Finish();
            }
            else
            {
                StartActivity(typeof(LoginActivity));
                Finish();
            }
        }
    }
}

