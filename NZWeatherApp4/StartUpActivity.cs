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

namespace NZWeatherApp4
{
    [Activity(Label = "Welcome to The Temp App", MainLauncher = true, Icon = "@drawable/icon")]
    public class StartUpActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //MainActivity won't start by itself, it needs a kick. 
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
            // Create your application here
        }
    }
}