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

namespace NZWeatherApp4 {
   
    public class Request {
        public string type { get; set; }
        public string query { get; set; }
        }

    public class WeatherIconUrl {
        public string value { get; set; }
        }

    public class WeatherDesc {
        public string value { get; set; }
        }

    public class CurrentCondition {
        public string observation_time { get; set; }
        public string temp_C { get; set; }
        public string temp_F { get; set; }
        public string weatherCode { get; set; }
        public List<WeatherIconUrl> weatherIconUrl { get; set; }
        public List<WeatherDesc> weatherDesc { get; set; }
        public string windspeedMiles { get; set; }
        public string windspeedKmph { get; set; }
        public string winddirDegree { get; set; }
        public string winddir16Point { get; set; }
        public string precipMM { get; set; }
        public string humidity { get; set; }
        public string visibility { get; set; }
        public string pressure { get; set; }
        public string cloudcover { get; set; }
        }

    public class WeatherIconUrl2 {
        public string value { get; set; }
        }

    public class WeatherDesc2 {
        public string value { get; set; }
        }

    public class Weather {
        public string date { get; set; }
        public string tempMaxC { get; set; }
        public string tempMaxF { get; set; }
        public string tempMinC { get; set; }
        public string tempMinF { get; set; }
        public string windspeedMiles { get; set; }
        public string windspeedKmph { get; set; }
        public string winddirection { get; set; }
        public string winddir16Point { get; set; }
        public string winddirDegree { get; set; }
        public string weatherCode { get; set; }
        public List<WeatherIconUrl2> weatherIconUrl { get; set; }
        public List<WeatherDesc2> weatherDesc { get; set; }
        public string precipMM { get; set; }
        }

    public class Data {
        public List<Request> request { get; set; }
        public List<CurrentCondition> current_condition { get; set; }
        public List<Weather> weather { get; set; }
        }

    public class RootObject {
        public Data data { get; set; }
        }
    }
//http://developer.worldweatheronline.com/api/code-examples.aspx 
//THE URL
//http://api.worldweatheronline.com/free/v1/weather.ashx?q=-43.526429,172.637637&format=json&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
//THE DATA

//{"data":{"request":[{"type":"LatLon","query":"Lat -43.53 and Lon 172.64"}],"current_condition":[{"observation_time":"10:58 PM","temp_C":"18","temp_F":"64","weatherCode":"113","weatherIconUrl":[{"value":"http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png"}],"weatherDesc":[{"value":"Sunny"}],"windspeedMiles":"11","windspeedKmph":"17","winddirDegree":"30","winddir16Point":"NNE","precipMM":"0.0","humidity":"37","visibility":"10","pressure":"1005","cloudcover":"0"}],"weather":[{"date":"2016-05-16","tempMaxC":"23","tempMaxF":"73","tempMinC":"9","tempMinF":"48","windspeedMiles":"26","windspeedKmph":"42","winddirection":"NNW","winddir16Point":"NNW","winddirDegree":"335","weatherCode":"353","weatherIconUrl":[{"value":"http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0009_light_rain_showers.png"}],"weatherDesc":[{"value":"Light rain shower"}],"precipMM":"0.5"}]}}

//STUCK IT INTO HERE
//http://json2csharp.com/#
//   }