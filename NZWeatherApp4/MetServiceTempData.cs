using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Net;
using ModernHttpClient;

namespace NZWeatherApp4
{
    public static class MetServiceTempData
    {

        private static string URL { get; set; }
        public static string City { get; set; } = "christchurch";
        private static string ImageUrl { get; set; }
        public static string TempText { get; set; }
        public static Bitmap TempImage { get; set; }
        private static string DownloadedString { get; set; }
        public static string ErrorMessage { get; set; }
        public static string WidgetTemp { get; set; }






        public static string GetCity(Spinner Sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = Sender; //make a fake spinner and send through the data to it

            City = spinner.GetItemAtPosition(e.Position).ToString();

            City = City.ToLower(); //make it lower case for the URL to work
            return City;
        }

        public static void WidgetTempDownload()
        {//I need a non async variable returns otherwise I get this error
            //http://stackoverflow.com/questions/14658001/cannot-implicitly-convert-type-string-to-system-threading-tasks-taskstring

            var s = TempDownload();
            //  WidgetTemp = s.Result;
        }

        public static async Task<string> TempDownload()
        {
            //Create the URL - we can change this later for other places
            // URL = "http://m.metservice.com/towns/christchurch";
            URL = "http://m.metservice.com/towns/" + City;
            //run the method that dl's the temp

            //download the website as a string. https://developer.xamarin.com/recipes/ios/network/web_requests/download_a_file/

            //downloads the string and returns it
            var webaddress = new Uri(URL); //Get the URL change it to a Uniform Resource Identifier
            //import ModernHTTPClient library to get this working
            var httpClient = new HttpClient(new NativeMessageHandler());

            Task<string> contentsTask = httpClient.GetStringAsync(webaddress); // async method!

            // await! control returns to the caller and the task continues to run on another thread
            DownloadedString = await contentsTask;
            return await ProcessTextFromSite(DownloadedString);
        }

        private static async Task<string> ProcessTextFromSite(string contents)
        {

            ParseCDATA.ExtractOutCDATA(DownloadedString);



            // try {
            //   string StrMetService = contents; //  Result is a property that holds the DL'ed string

            //get rid of single quotes in the string, its a pain otherwise. Always do this first
            //   StrMetService = StrMetService.Replace("\"", string.Empty);

            //get rid of everything in the header, you don't need it
            //    StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<!--//--><![CDATA[//><!--"));

            var Temp = CDATATemp.adCurrentTemp;// ExtractTheTemperatureFromString(StrMetService);
            //read in the Old temp if it exists
            string OldTemp = ReadText("Temperature.txt");
            //Save the New temp as the old temp
            SaveText("Temperature.txt", Temp);
            //   GetImageBitmapFromUrl(ExtractImagePath(DownloadedString));

            //Yes!!! widget temp comes from here. 
            WidgetTemp = Temp + "c " + "Old Temp " + OldTemp + "c ";

            return Temp + "c " + "Old Temp " + OldTemp + "c ";
        }


        private static void webclient_DownloadStringCompleted(object Sender, DownloadStringCompletedEventArgs e)
        {
            //http://stackoverflow.com/questions/30634329/how-to-download-image-from-url-in-xamarin-android
            try
            {
                string StrMetService = e.Result; //  Result is a property that holds the DL'ed string

                //get rid of single quotes in the string, its a pain otherwise. Always do this first
                StrMetService = StrMetService.Replace("\"", string.Empty);


                //get rid of everything in the header, you don't need it
                StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body>"));
                //get the left hand side of where the temp is, add 30 to get to the end of this string and the beginning of the number
                var intTempLeft = StrMetService.IndexOf("summary top><div class=ul><h2>") + 30;
                //get the legth of the temp string you want. To do that find the text after the Temp and subtrack the length BEFORE the temp from it.
                var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;

                //Pass all the text to the textView in the Scroll bar so you can see the text
                //FindViewById<TextView>(Resource.Id.AllText).Text = StrMetService;
                //Pass the Temp to the TempText TextView
                var Temp = StrMetService.Substring(intTempLeft, intTempRight);
                //read in the Old temp if it exists
                string OldTemp = ReadText("Temperature.txt");
                //show it on the View
                TempText = Temp + " c " + "Old Temp " + OldTemp + " c ";
                //save the new temp
                SaveText("Temperature.txt", Temp);
                //Run the Image code, and pass the image to the ImageView
                // TempImage = GetImageBitmapFromUrl(ExtractImagePath(e.Result));
                //  myImageView.SetImageBitmap(imageBitmap);

            }
            catch (Exception)
            {

                //  Toast.MakeText(this, "Not working, Why!!!!!???????", ToastLength.Long).Show();
            }



        }
        private static void SaveText(string filename, string text)
        {

            //https://developer.xamarin.com/guides/xamarin-forms/working-with/files/
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);

        }
        private static string ReadText(string filename)
        {
            //if the file exists then read from it
            string text;
            // could use https://developer.xamarin.com/api/member/System.IO.File.Exists/p/System.String/   

            //set up the directory path and combine it with the file name
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            //if the file exists 
            if (File.Exists(filePath))
            {

                return System.IO.File.ReadAllText(filePath);
            }
            else
            {
                //otherwise throw a message
                text = "";
                //  Toast.MakeText(this, "No File", ToastLength.Short).Show();

            }
            return text;
        }

        //Welcome to learning from the Internet to DL an image 

        public static async Task<Bitmap> GetImageBitmapFromUrl()
        {
            //http://haseeb-ahmed.com/blog/2015/03/image-from-url-in-imageview-xamarin/ 
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(ImageUrl);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            //scale the bitmap to make it bigger
            //https://docs.xamarin.com/api/member/Android.Graphics.Bitmap.CreateScaledBitmap/p/Android.Graphics.Bitmap/System.Int32/System.Int32/System.Boolean/ and https://forums.xamarin.com/discussion/7153/bitmap-resizing
            var bitmapScaled = Bitmap.CreateScaledBitmap(imageBitmap, 200, 200, true);
            imageBitmap.Recycle();


            return bitmapScaled;
        }

        public static void ExtractImagePath()
        {
            string StrMetService = DownloadedString;
            //Return back the path to the image only
            StrMetService = StrMetService.Replace("\"", string.Empty);

            //</div><div class="mob-page" id="forecasts-block"><h2>10 Day Forecast</h2><div class="item"><img src="/sites/all/themes/mobile/images-new/wx-icons/showers_wht.gif" width="32" height="32" title="Showers" alt="Showers" />

            var intImageLeft = StrMetService.IndexOf("images-new/wx-icons/") + 20;
            //add 30 to get to the end of this string and the beginning of the number
            var intImageCount = StrMetService.IndexOf("width=32 height=32") - intImageLeft;
            //the text on the right of the number
            var strImage = StrMetService.Substring(intImageLeft, intImageCount);

            ImageUrl = "http://m.metservice.com/sites/all/themes/mobile/images-new/wx-icons/" + strImage;
        }

    }
}