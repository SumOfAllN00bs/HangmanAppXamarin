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

namespace HangmanApp
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText edt_Username;
        Button btn_Login;
        Database db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            db = new Database();
            edt_Username = FindViewById<EditText>(Resource.Id.edt_Username);
            btn_Login = FindViewById<Button>(Resource.Id.btn_Login);

            btn_Login.Click += Btn_Login_Click;
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(edt_Username.Text) || edt_Username.Text.Length < 1)
            {
                Toast.MakeText(this, "Please enter a valid username", ToastLength.Short).Show();
                return;
            }
            db.Login(edt_Username.Text);
            StartActivity(typeof(GameActivity));
            Finish();
        }
    }
}