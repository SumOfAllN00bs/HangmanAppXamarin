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
using Android.Graphics.Drawables;

namespace HangmanApp
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        Database db = new Database();
        TextView txt_WordDisplay;
        string WordToGuess;
        AnimationDrawable BackGroundAnimation;
        ImageView img_Hang;
        List<int> HangingImageResources;
        LinearLayout qRow;
        LinearLayout aRow;
        LinearLayout zRow;
        List<Button> QwertyList;
        ImageView bckGround;
        int difficulty;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);
            string dbPath = Helper.GetLocalFilePath("words.db3");
            difficulty = db.CurrentOptions(this).Difficulty;
            Database db_Words = new Database(dbPath);
            WordToGuess = db_Words.GetRandomWord(difficulty);
            QwertyList = new List<Button>();
            txt_WordDisplay = FindViewById<TextView>(Resource.Id.txt_WordDisplay);
            qRow = FindViewById<LinearLayout>(Resource.Id.ll_Qrow);
            aRow = FindViewById<LinearLayout>(Resource.Id.ll_Arow);
            zRow = FindViewById<LinearLayout>(Resource.Id.ll_Zrow);
            bckGround = FindViewById<ImageView>(Resource.Id.img_GameBackground);
            img_Hang = FindViewById<ImageView>(Resource.Id.img_Hanging);

            //Toast.MakeText(this, "Difficulty: " + db.CurrentOptions(this).Difficulty, ToastLength.Long).Show();
            Toast.MakeText(this, WordToGuess, ToastLength.Long).Show();

            txt_WordDisplay.Text = string.Join("", Enumerable.Repeat("_ ", WordToGuess.Length).ToArray());
            HangingImageResources = new List<int>();
            switch (difficulty)
            {
                case 1://easy
                    HangingImageResources.Add(Resource.Drawable.Hang1); HangingImageResources.Add(Resource.Drawable.Hang2);
                    HangingImageResources.Add(Resource.Drawable.Hang3); HangingImageResources.Add(Resource.Drawable.Hang4);
                    HangingImageResources.Add(Resource.Drawable.Hang5); HangingImageResources.Add(Resource.Drawable.Hang6);
                    HangingImageResources.Add(Resource.Drawable.Hang7); HangingImageResources.Add(Resource.Drawable.Hang8);
                    HangingImageResources.Add(Resource.Drawable.Hang9);
                    break;
                case 2://normal
                    HangingImageResources.Add(Resource.Drawable.Hang2); HangingImageResources.Add(Resource.Drawable.Hang4);
                    HangingImageResources.Add(Resource.Drawable.Hang6); HangingImageResources.Add(Resource.Drawable.Hang8);
                    HangingImageResources.Add(Resource.Drawable.Hang9);
                    break;
                case 3://hard
                    HangingImageResources.Add(Resource.Drawable.Hang3);
                    HangingImageResources.Add(Resource.Drawable.Hang5);
                    HangingImageResources.Add(Resource.Drawable.Hang7);
                    HangingImageResources.Add(Resource.Drawable.Hang9);
                    break;
                default:
                    break;
            }
            foreach (LinearLayout item in new List<LinearLayout>() { qRow, aRow, zRow })
            {
                string rowOfCharacters = "";
                if (item == qRow) rowOfCharacters = "qwertyuiop";
                if (item == aRow) rowOfCharacters = "asdfghjkl";
                if (item == zRow) rowOfCharacters = "zxcvbnm";

                foreach (char c in rowOfCharacters)
                {
                    Button btn_newButton = new Button(this);
                    btn_newButton.Text = c.ToString().ToUpper();
                    LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    param.SetMargins(1, 2, 0, 1);
                    item.AddView(btn_newButton, param);
                    btn_newButton.Click += LetterClicked;
                    QwertyList.Add(btn_newButton);
                }
                item.SetGravity(GravityFlags.CenterHorizontal);
                bckGround.SetImageResource(Resource.Drawable.BackGround);

            }
        }

        private void LetterClicked(object sender, EventArgs e)
        {
            if (WordToGuess.ToLower().Contains((sender as Button).Text.ToLower()))
            {
                //code gets a little dense and abstract
                /*
                 * we need to create a new version of the displayed text where anything that hasn't yet
                 * been guessed is still hidden by an underscore and where each of the letters is separated 
                 * by a space if letters have been already guessed we need to preserve them into the new display
                 * and if new letters are guessed by the pressing of this button then we need to change the display
                 * to reflect that
                 */
                string tmp = "";
                for (int i = 0; i < WordToGuess.Length; i++)                                            //for every letter in the word to be guessed
                {
                    if (txt_WordDisplay.Text[i*2] == '_')                                               //check if we have already guessed it
                    {
                        if (WordToGuess[i].ToString().ToLower() == (sender as Button).Text.ToLower())   //  check if this is a successful guess in this spot
                        {
                            tmp = tmp + (sender as Button).Text.ToUpper() + ' ';                        //      preserve the spacing and use the button to store the correct guess
                        }
                        else                                                                            //  this was unsuccessful so just keep hidden and move on
                        {
                            tmp = tmp + "_ ";
                        }
                    }
                    else                                                                                //We have already guessed it just ad it to the display
                    {
                        tmp = tmp + txt_WordDisplay.Text[i * 2] + ' ';
                    }
                }

                txt_WordDisplay.Text = tmp;
                if (!tmp.Contains("_"))
                {
                    //all characters have now been correctly guessed
                }
                //before leaving this we have to disable the button because there is no sense in letting the player guess the same guess
                (sender as Button).Enabled = false;
            }
            else
            {
                //the word didn't have this letter
                bckGround.SetImageDrawable(BackGroundAnimation);
                BackGroundAnimation = (AnimationDrawable) Resources.GetDrawable(Resource.Drawable.AnimationMistake);
                BackGroundAnimation.Start();
                (sender as Button).Enabled = false;
            }
        }
    }
}