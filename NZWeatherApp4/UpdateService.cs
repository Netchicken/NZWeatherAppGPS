using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NZWeatherApp4
{
    //http://forums.xamarin.com/discussion/22941/how-do-i-include-in-my-app-widget
    //http://www.vogella.com/tutorials/AndroidWidgets/article.html

    [Service]
    class UpdateService : Service
    {

        public override void OnStart(Intent intent, int startId)
        {
            // Build the widget update for today
            RemoteViews updateViews = buildUpdate(this);

            // Push update for this widget to the home screen
            ComponentName thisWidget = new ComponentName(this, Java.Lang.Class.FromType(typeof(TempWidget)).Name);

            AppWidgetManager manager = AppWidgetManager.GetInstance(this);
            manager.UpdateAppWidget(thisWidget, updateViews);
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            return null;
        }



        public RemoteViews buildUpdate(Context context)
        {
            //Temptext not used, just to run method
            //https://forums.xamarin.com/discussion/58364/how-do-i-get-my-buttons-on-my-homescreen-widget-to-do-something

            //get the temp from the class this just runs the method not returning data.
            var notused = MetServiceTempData.TempDownload();
            //null-conditional operator : http://www.codeproject.com/Tips/1023426/Whats-New-in-Csharp
            string FinalTemp = MetServiceTempData.WidgetTemp ?? "No Temp";


            // Build an update that holds the updated widget contents
            var updateViews = new RemoteViews(context.PackageName, Resource.Layout.widget);

            //Update the views on the widget
            updateViews.SetTextViewText(Resource.Id.TempText, FinalTemp);
            updateViews.SetTextViewText(Resource.Id.City, MetServiceTempData.City);

            // When user CLICKS on widget, go to website

            //create an intent and the click event for the widget
            Intent defineIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://www.metservice.com/towns-cities/christchurch/christchurch"));

            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, defineIntent, 0);

            //does the updating
            updateViews.SetOnClickPendingIntent(Resource.Id.TempText, pendingIntent);
            //  }

            return updateViews;
        }
        //https://forums.xamarin.com/discussion/483/home-screen-widget-refresh-possible
        private void UpdateWidget()
        {
            //RemoteViews updateViews = CreateWidgetView();
            //// the name is usually something like myapp.MyCoolWidget
            //ComponentName statusWidget =  new ComponentName(this, FetchWidget);
            //AppWidgetManager manager = AppWidgetManager.GetInstance(this);
            //manager.UpdateAppWidget(statusWidget, updateViews);
        }
       
        private RemoteViews CreateWidgetView()
        {
            RemoteViews views = new RemoteViews(PackageName, Resource.Layout.TempText);
            views.SetTextViewText(Resource.Id.TempText, "TEST MESSAGE");
            return views;
        }


 //https://forums.xamarin.com/discussion/22355/widget-textview-not-updating
        //need to make the namespace lower case to be able to use it


        private ComponentName FetchWidget(Type widgetType, Context context)
        {
            ComponentName retWidget = null;
            if (widgetType != null) {
                string widgetNamespace = widgetType.Namespace.ToLowerInvariant();
                string typeName = typeof(TempWidget).Name;
                retWidget = new ComponentName(context, widgetNamespace + "." + typeName);
            }
            return retWidget;
        }


    }

}