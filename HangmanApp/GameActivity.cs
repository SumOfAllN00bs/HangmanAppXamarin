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
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        Database db = new Database();
        LinearLayout qRow;
        LinearLayout aRow;
        LinearLayout zRow;
        List<Button> QwertyList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);
            QwertyList = new List<Button>();
            qRow = FindViewById<LinearLayout>(Resource.Id.ll_Qrow);
            aRow = FindViewById<LinearLayout>(Resource.Id.ll_Arow);
            zRow = FindViewById<LinearLayout>(Resource.Id.ll_Zrow);
            //Toast.MakeText(this, "Difficulty: " + db.CurrentOptions(this).Difficulty, ToastLength.Long).Show();

            foreach (LinearLayout item in new List<LinearLayout>() { qRow, aRow, zRow })
            {
                string rowOfCharacters = "";
                if (item == qRow)
                {
                    rowOfCharacters = "qwertyuiop";
                }
                if (item == aRow)
                {
                    rowOfCharacters = "asdfghjkl";
                }
                if (item == zRow)
                {
                    rowOfCharacters = "zxcvbnm";
                }
                foreach (char c in rowOfCharacters)
                {
                    Button btn_newButton = new Button(this);
                    btn_newButton.Text = c.ToString().ToUpper();
                    LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    param.SetMargins(1, 2, 0, 1);
                    param.Weight = 1;
                    item.AddView(btn_newButton, param);
                    QwertyList.Add(btn_newButton);
                }
                item.SetGravity(GravityFlags.CenterHorizontal);
            }
        }
    }
}