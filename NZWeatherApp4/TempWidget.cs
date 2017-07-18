using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NZWeatherApp4
{
    [BroadcastReceiver(Label = "NZ Weather")] //name of widget
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", "android.appwidget.action.APPWIDGET_ENABLED", "android.appwidget.action.ACTION_APPWIDGET_PICK" })]//Sent when it is time to update your AppWidget.  
    [MetaData("android.appwidget.provider", Resource = "@xml/widget")]
    class TempWidget : AppWidgetProvider
    {


        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            //service name of the service is UpdateService
            context.StartService(new Intent(context, typeof(UpdateService)));
        }

      
    }
}