using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HangmanApp
{
    public class Helper
    {
        public static void SetFonts(AssetManager Assets,List<View> controls)
        {
            var font = Typeface.CreateFromAsset(Assets, "iosevka-regular.ttf");
            foreach (View V in controls)
            {
                (V as TextView).Typeface = font;
            }
        }
    }
}