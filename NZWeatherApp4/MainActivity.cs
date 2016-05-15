using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;


namespace NZWeatherApp4 {
    [Activity(Label = "NZWeatherApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener {
        private Button btnGetWeather;
        private Button btnGPS;
        private string Lat;
        private string Lon;
        LocationManager locMgr;
        string locationProvider;
        private Location CurrentLocation;
        string locProv;
        TextView GPSText;
        TextView AllText;
        TextView TempText;
        private string City = "christchurch"; //default city
        private ImageView myImageView;
        private string StrMetService;
        public string URL { get; set; }

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource  .....
            SetContentView(Resource.Layout.Main);
            //tie in the ImageView
            myImageView = FindViewById<ImageView>(Resource.Id.Image);
            GPSText = FindViewById<TextView>(Resource.Id.txtGPS);
            btnGPS = FindViewById<Button>(Resource.Id.btnGPS);
            AllText = FindViewById<TextView>(Resource.Id.AllText);
            TempText = FindViewById<TextView>(Resource.Id.TempText);
            SpinnerSetup();

            //Tie in the Button, and create a Click event using a delegate (Not the old fashioned way, but MS likes it. )
            btnGetWeather = FindViewById<Button>(Resource.Id.GetWeatherButton);
            //When you click the button
            btnGetWeather.Click += btnGetWeather_Click; //Pink color means its an event
            btnGPS.Click += BtnGPS_Click;
            InitializeLocationManager();

            }

        async private void BtnGPS_Click(object sender, EventArgs e) {

            if (CurrentLocation == null) {
                AllText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
                }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);


            }
        protected override void OnResume() {
            base.OnResume();

            // initialize location manager again 
            locMgr.RequestLocationUpdates(locationProvider, 0, 0, this);
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            }
        //https://developer.xamarin.com/recipes/android/os_device_resources/gps/get_current_device_location/
        //http://developer.android.com/guide/topics/location/strategies.html

        void InitializeLocationManager() {
            //The LocationManager class will listen for GPS updates from the device and notify the application by way of events. In this example we ask Android for the best location provider that matches a given set of Criteria and provide that provider to LocationManager.
            locMgr = (LocationManager)GetSystemService(LocationService);


            //Define a Criteria for the best location provider
            Criteria criteriaForLocationService = new Criteria
                {
                //A constant indicating an approximate accuracy
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
                };
            //gets the best providor
            locationProvider = locMgr.GetBestProvider(criteriaForLocationService, true);
            Toast.MakeText(this, "Using " + locationProvider, ToastLength.Short).Show();

            }
        //ILocationListener methods
        void ILocationListener.OnLocationChanged(Location location) {
            CurrentLocation = location;
            UpdateGPSLocation();
            }

        private void UpdateGPSLocation() {
            Lat = CurrentLocation.Latitude.ToString();
            Lon = CurrentLocation.Longitude.ToString();
            Toast.MakeText(this, "Lat " + Lat + "Lon " + Lon, ToastLength.Long).Show();
            //json  http://api.worldweatheronline.com/free/v1/weather.ashx?q=-43.526429,172.637637&format=json&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
            //xml  http://api.worldweatheronline.com/free/v1/weather.ashx?q=" & myGPS.Lat & "," & myGPS.Lon & "&format=xml&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
            GPSText.Text = "Lat " + Lat + "Lon " + Lon; // just so we know it exists
            }
        //Turn off GPS?
        void ILocationListener.OnProviderDisabled(string provider) {
            throw new NotImplementedException();
            }

        void ILocationListener.OnProviderEnabled(string provider) {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Short).Show();
            }

        void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras) {
            throw new NotImplementedException();
            }

        async Task<Address> ReverseGeocodeCurrentLocation() {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(CurrentLocation.Latitude, CurrentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
            }

        void DisplayAddress(Address address) {
            if (address != null) {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++) {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                    }
                AllText.Text = deviceAddress.ToString();
                } else {
                AllText.Text = "Unable to determine the address. Try again in a few minutes.";
                }
            }



        protected override void OnPause() {
            base.OnPause();
            locMgr.RemoveUpdates(this);

            }

        //===============================================================================================



        private void btnGetWeather_Click(object sender, EventArgs e) {
            //Create the URL - we can change this later for other places
            // URL = "http://m.metservice.com/towns/christchurch";
            URL = "http://m.metservice.com/towns/" + City;
            //run the method that dl's the temp
            ConnectToNetAndDLTemp();
            //change the text on the button, so that you know something has happened

            btnGetWeather.Text = City.ToUpper();
            }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) {
            var spinner = (Spinner)sender; //make a fake spinner and send through the data to it

            City = spinner.GetItemAtPosition(e.Position).ToString();

            City = City.ToLower(); //make it lower case for the URL to work
            var toast = string.Format("The city is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
            }

        public void ConnectToNetAndDLTemp() {
            //download the website as a string. https://developer.xamarin.com/recipes/ios/network/web_requests/download_a_file/
            try {

                //downloads the string and returns it
                var webaddress = new Uri(URL); //Get the URL change it to a Uniform Resource Identifier
                var webclient = new WebClient(); //Make a webclient to dl stuff ......

                webclient.DownloadStringAsync(webaddress); //dl the website 
                                                           //Pink color means its an event
                webclient.DownloadStringCompleted += webclient_DownloadStringCompleted;
                //Connect a method to the run when the DL is finished, 
                } catch (Exception e) {
                var toast = string.Format("Something went wrong probably no city " + e.Message);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
                }
            }

        private void webclient_DownloadStringCompleted(object Sender, DownloadStringCompletedEventArgs e) {
            //http://stackoverflow.com/questions/30634329/how-to-download-image-from-url-in-xamarin-android
            try {
                StrMetService = e.Result; //  Result is a property that holds the DL'ed string

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
                TempText.Text = Temp + " c " + "Old Temp " + OldTemp + " c ";
                //save the new temp
                SaveText("Temperature.txt", Temp);
                //Run the Image code, and pass the image to the ImageView
                var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath());
                myImageView.SetImageBitmap(imageBitmap);

                } catch (Exception) {

                Toast.MakeText(this, "Not working, Why!!!!!???????", ToastLength.Long).Show();
                }


            //  }
            }

        //Welcome to learning from the Internet to DL an image 

        private Bitmap GetImageBitmapFromUrl(string url) {
            //http://haseeb-ahmed.com/blog/2015/03/image-from-url-in-imageview-xamarin/ 
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient()) {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0) {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

            //scale the bitmap to make it bigger
            //https://docs.xamarin.com/api/member/Android.Graphics.Bitmap.CreateScaledBitmap/p/Android.Graphics.Bitmap/System.Int32/System.Int32/System.Boolean/ and https://forums.xamarin.com/discussion/7153/bitmap-resizing
            var bitmapScaled = Bitmap.CreateScaledBitmap(imageBitmap, 200, 200, true);
            imageBitmap.Recycle();


            return bitmapScaled;
            }

        public string ExtractImagePath() {
            //Return back the path to the image only
            StrMetService = StrMetService.Replace("\"", string.Empty);

            //</div><div class="mob-page" id="forecasts-block"><h2>10 Day Forecast</h2><div class="item"><img src="/sites/all/themes/mobile/images-new/wx-icons/showers_wht.gif" width="32" height="32" title="Showers" alt="Showers" />

            var intImageLeft = StrMetService.IndexOf("images-new/wx-icons/") + 20;
            //add 30 to get to the end of this string and the beginning of the number
            var intImageCount = StrMetService.IndexOf("width=32 height=32") - intImageLeft;
            //the text on the right of the number
            var strImage = StrMetService.Substring(intImageLeft, intImageCount);

            return "http://m.metservice.com/sites/all/themes/mobile/images-new/wx-icons/" + strImage;
            }

        public void SaveText(string filename, string text) {

            //https://developer.xamarin.com/guides/xamarin-forms/working-with/files/
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);

            }
        public string ReadText(string filename) {
            //if the file exists then read from it
            string text;
            // could use https://developer.xamarin.com/api/member/System.IO.File.Exists/p/System.String/   

            //set up the directory path and combine it with the file name
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            //if the file exists 
            if (File.Exists(filePath)) {

                return System.IO.File.ReadAllText(filePath);
                } else {
                //otherwise throw a message
                text = "";
                Toast.MakeText(this, "No File", ToastLength.Short).Show();

                }
            return text;
            }

        private void SpinnerSetup() {
            //https://developer.xamarin.com/guides/android/user_interface/spinner/ 
            //tie in the spinner
            var spinner = FindViewById<Spinner>(Resource.Id.spCity);

            spinner.ItemSelected += spinner_ItemSelected; //tie it to the method.
            //The CreateFromResource() method then creates a new ArrayAdapter, which binds each item in the string array to the initial appearance for the Spinner (which is how each item will appear in the spinner when selected). 
            var arrayadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.place_array,
                Android.Resource.Layout.SimpleSpinnerItem);
            //SetDropDownViewResource is called to define the appearance for each item when the widget is opened (SimpleSpinItem is another standard layout defined by the platform) 
            arrayadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //Finally, the ArrayAdapter is set to associate all of its items with the Spinner by setting the Adapter property
            spinner.Adapter = arrayadapter;
            }


        }
    }