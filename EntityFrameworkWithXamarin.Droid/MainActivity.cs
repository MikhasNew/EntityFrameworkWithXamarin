using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using EntityFrameworkWithXamarin.Core;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Android.Content;

namespace EntityFrameworkWithXamarin.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Button button = FindViewById<Button>(Resource.Id.buttonStart);

            // button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            TextView textView = FindViewById<TextView>(Resource.Id.TextView1);



            var dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var fileName = "Cats.db";
            var dbFullPath = Path.Combine(dbFolder, fileName);
            
            var startServiceIntent = new Intent(this, typeof(ForegraundService));
            startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                    StartForegroundService(startServiceIntent);
                else
                    StartService(startServiceIntent);
           

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}