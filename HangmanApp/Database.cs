using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;

namespace HangmanApp
{
    public class Database
    {
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dbHangman.db3");

        public Database()
        {
        }

        public bool IsLoggedIn()
        {
            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<Options>();
            var table = db.Table<Options>();
            if (table.Count() < 1)
            {
                db.Insert(new Options());
            }
            return table.FirstOrDefault().IsLoggedIn;
        }

        public Account LoggedInAccount(Context activity)
        {
            if (IsLoggedIn())
            {
                SQLiteConnection db = new SQLiteConnection(dbPath);
                db.CreateTable<Options>();
                var table = db.Table<Options>();
                int? loggedInID = table.FirstOrDefault().LoggedInAccountID;
                if (loggedInID == null)
                {
                    Toast.MakeText(activity, "Error: no Id Stored", ToastLength.Short).Show();
                    return null;
                }
                var accountTable = db.Table<Account>();
                
                return accountTable.Where(acT => acT.ID == loggedInID).FirstOrDefault();
            }
            else
            {
                Console.Write("Error: NotLoggedIn");
                Toast.MakeText(activity, "Error: NotLoggedIn", ToastLength.Short).Show();
                return null;
            }
        }
        public Options CurrentOptions(Context activity)
        {
            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<Options>();
            var table = db.Table<Options>();
            Options _options = table.FirstOrDefault();
            return _options;
        }
        public bool Login(string username, int difficulty) //perform the action of logging in
        {
            SQLiteConnection db = new SQLiteConnection(dbPath);
            db.CreateTable<Account>();
            var table = db.Table<Account>();
            //first check if account already exists
            bool check = false;
            Account logInAccount = null;
            foreach (Account A in table)
            {
                if (A.Username == username)
                {
                    check = true;
                    logInAccount = A;
                    break;
                }
            }
            if (check && logInAccount != null)
            {
                //account already exists just load it
                //deal with game options. 1 copy of options should be id 1 and will be default options for game when loading an existing profile the options stored in profile are loaded
                db.CreateTable<Options>();
                var optionsTable = db.Table<Options>();
                Options o = optionsTable.FirstOrDefault();
                logInAccount.AccountOptions = db.Get<Options>(opt => opt.ID == logInAccount.AccountOptionsID);
                o.Difficulty = logInAccount.AccountOptions.Difficulty;
                /*
                 * Difficulty is loaded from saved account
                 * don't know if this behaviour is what I want because it may be the case that
                 * the user has an account which has a difficulty saved but chooses a new difficulty
                 * be choosing to log in using another account but keep the difficulty they chose
                 * this won't happen if you load the difficulty from the account that is saved
                 * when you choose to log in
                 */
                o.IsLoggedIn = true;//I probably don't need this anymore I just have to
                o.LoggedInAccountID = logInAccount.ID;
                db.Update(o);
                return true;
            }
            else if (logInAccount == null)
            {
                //the following is a little convoluted I honestly don't know if there is a simpler way to do this
                //account doesn't exist create new one then load it
                logInAccount = new Account();
                logInAccount.AccountOptions = new Options();
                logInAccount.HighestScore = 0;
                logInAccount.Username = username;
                db.Insert(logInAccount);

                //deal with game options. 1 copy of options should be id 1 and will be default options for game when loading an existing profile the options stored in profile are loaded
                db.CreateTable<Options>();
                var optionsTable = db.Table<Options>();

                //default options will always be first because it should be automatically inserted when the app is first run
                Options o = optionsTable.FirstOrDefault(); 
                o.Difficulty = difficulty;
                o.IsLoggedIn = true;
                logInAccount.AccountOptions.IsLoggedIn = true;
                logInAccount.AccountOptions.LoggedInAccountID = logInAccount.ID;

                o.LoggedInAccountID = logInAccount.ID;
                db.Update(o);

                //finally insert options created in new account
                int newAccountOptionsID = db.Insert(logInAccount.AccountOptions);
                logInAccount.AccountOptionsID = newAccountOptionsID;
                db.Update(logInAccount); // after already inserting the new account I have to update it aftwards with the id of the accountoptions that I get back after inserting the account options
                //there has got to be a better way :)

                return false; //false just means account wasn't found.
            }
            throw new Exception("Unexpected Code Path");
            return false;
        }
    }
}