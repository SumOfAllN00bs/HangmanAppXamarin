using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using SQLite;
using Android.Graphics;

namespace HangmanApp
{
    [Activity(Label = "HangmanApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView txt_Welcome;
        Button btn_Start;
        Database db;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            db = new Database();
            txt_Welcome = FindViewById<TextView>(Resource.Id.txt_Welcome);
            btn_Start = FindViewById<Button>(Resource.Id.btn_Start);

            var font = Typeface.CreateFromAsset(Assets, "iosevka-regular.ttf");
            if (db.IsLoggedIn())
            {
                txt_Welcome.Text = "Welcome " + db.LoggedInAccount(this).Username + " To The HangMan Game";
            }
            btn_Start.Typeface = font;
            btn_Start.Click += Btn_Start_Click;
        }

        private void Btn_Start_Click(object sender, System.EventArgs e)
        {
            if (db.IsLoggedIn())
            {
                StartActivity(typeof(GameActivity));
            }
            else
            {
                StartActivity(typeof(LoginActivity));
            }
        }
    }
}

