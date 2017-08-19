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

            foreach (char c in "qwertyuiop")
            {
                Button btn_newButton = new Button(this);
                btn_newButton.Text = c.ToString().ToUpper();
                LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(1, 5, 0, 5);
                qRow.AddView(btn_newButton, param);
                QwertyList.Add(btn_newButton);
            }
            foreach (char c in "asdfghjkl")
            {
                Button btn_newButton = new Button(this);
                btn_newButton.Text = c.ToString().ToUpper();
                LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(1, 5, 0, 5);
                aRow.AddView(btn_newButton, param);
                QwertyList.Add(btn_newButton);
            }
            foreach (char c in "zxcvbnm")
            {
                Button btn_newButton = new Button(this);
                btn_newButton.Text = c.ToString().ToUpper();
                LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(1, 5, 0, 5);
                zRow.AddView(btn_newButton, param);
                QwertyList.Add(btn_newButton);
            }
            qRow.SetGravity(GravityFlags.CenterHorizontal);
            aRow.SetGravity(GravityFlags.CenterHorizontal);
            zRow.SetGravity(GravityFlags.CenterHorizontal);
        }
    }
}