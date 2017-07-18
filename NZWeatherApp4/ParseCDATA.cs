using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NZWeatherApp4
{
    static class CDATATemp
    {
        public static string ad_forecast24 { get; set; }
        public static string ad_forecast48 { get; set; }
        public static string ad_maxTemp24 { get; set; }
        public static string ad_maxTemp48 { get; set; }
        public static string adCurrentTemp { get; set; }

    }

    //https://stackoverflow.com/questions/2784183/what-does-cdata-in-xml-mean
    //        <script type = "text/javascript" >
    //            < !--//--><![CDATA[//><!--
    //        var ad_forecast24 = "PARTLY CLOUDY",
    //            ad_forecast48 = "FEW SHOWERS",
    //            ad_maxTemp24 = "10",
    //            ad_maxTemp48 = "9",
    //            adSaturdayForecast = "Partly cloudy",
    //            adSundayForecast = "Cloudy",
    //            adCurrentTemp = "3.7";
    //        var mobile_tile_fcst_atf = true,
    //            mobile_tile_ski = false,
    //            mobile_tile_pollen = false;
    ////--><!]]>
    //            </script>
    static class ParseCDATA
    {
        public static void ExtractOutCDATA(string DL)
        {
            DL = DL.Replace("\"", string.Empty);
            int left = DL.IndexOf("ad_forecast24");
            int right = DL.IndexOf("var mobile_tile_fcst_atf");

            string extract = DL.Substring(left, right - left);

            string[] data = extract.Split(',');

            CDATATemp.ad_forecast24 = data[0].Substring(data[0].IndexOf('=') + 2).Trim();
            CDATATemp.ad_forecast48 = data[1].Substring(data[1].IndexOf('=') + 2).Trim();
            CDATATemp.ad_maxTemp24 = data[2].Substring(data[2].IndexOf('=') + 2).Trim();
            CDATATemp.ad_maxTemp48 = data[3].Substring(data[3].IndexOf('=') + 2).Trim();
            CDATATemp.adCurrentTemp = data[6].Substring(data[6].IndexOf('=') + 2).Trim();

            CDATATemp.adCurrentTemp = CDATATemp.adCurrentTemp.Replace(";", string.Empty);

        }




    }
}