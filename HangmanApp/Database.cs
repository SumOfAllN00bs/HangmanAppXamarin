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
        public bool Login(string username) //perform the action of logging in
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
                o.IsLoggedIn = true;
                o.LoggedInAccountID = logInAccount.ID;
                db.Update(o);
                return true;
            }
            else if (logInAccount == null)
            {
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
                o.Difficulty = logInAccount.AccountOptions.Difficulty;
                o.IsLoggedIn = true;
                logInAccount.AccountOptions.IsLoggedIn = true;
                logInAccount.AccountOptions.LoggedInAccountID = logInAccount.ID;

                o.LoggedInAccountID = logInAccount.ID;
                db.Update(o);

                //finally insert options created in new account
                int test = db.Insert(logInAccount.AccountOptions);
                return false; //false just means account wasn't found.
            }
            throw new Exception("Unexpected Code Path");
            return false;
        }
    }
}