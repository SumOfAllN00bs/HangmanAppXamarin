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
using Android.Views.Animations;
using System.Timers;

namespace HangmanApp
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        class timerState
        {
            public int Counter { get; set; }
            public Timer animTimer;
            public timerState()
            {
                Counter = 0;
            }
        }
        timerState ts = new timerState();
        Database db = new Database();
        TextView txt_WordDisplay;
        TextView txt_ScoreDisplay;
        string WordToGuess;
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
            txt_ScoreDisplay = FindViewById<TextView>(Resource.Id.txt_ScoreDisplay);
            qRow = FindViewById<LinearLayout>(Resource.Id.ll_Qrow);
            aRow = FindViewById<LinearLayout>(Resource.Id.ll_Arow);
            zRow = FindViewById<LinearLayout>(Resource.Id.ll_Zrow);
            bckGround = FindViewById<ImageView>(Resource.Id.img_GameBackground);
            img_Hang = FindViewById<ImageView>(Resource.Id.img_Hanging);
            ts.animTimer = new Timer(50); //50 means 20 frames per second 33 means 30 frames per second

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
                bckGround.SetBackgroundResource(Resource.Drawable.BackGround);

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
                ts.animTimer.Enabled = true;
                ts.animTimer.Elapsed += AnimTimer_Elapsed; ;
                ts.animTimer.Start();
                (sender as Button).Enabled = false;
            }
        }

        private void AnimTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //using animationDrawable and viewAnimation and Animation never seemed to work
            //I'd get all sorts of errors or visual glitches
            //so an animation using a simple timer seemed the most likely way to get animation to work
            //btw this animation is just a little warning to the player that an error occured
            if (ts.Counter >= 25) //framerate of 20 per second 20 = 1 sec roughly
            {
                ts.animTimer.Stop();
                ts.animTimer.Enabled = false;
                ts.Counter = 0;
                return;
            }
            ts.Counter++;
            if (ts.Counter % 2 == 0)
                RunOnUiThread(() => { bckGround.SetImageResource(Resource.Drawable.BackGround2); });
            else
                RunOnUiThread(() => { bckGround.SetImageResource(Resource.Drawable.BackGround); });
        }
    }
}
