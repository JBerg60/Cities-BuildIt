using System;

namespace BuildIt.Geo
{
    public class GeoTools
    {
        const double pi = Math.PI;

        public const double NauticalMile = 1851.85f; //1852; 2 * pi * EarthRadius * 1000 / 360 / 60; 
        public const double EarthRadius = NauticalMile * 60 * 360 / (2 * pi) / 1000; // 6378.137f radius of the earth in kilometer

        const double meter1 = 1 / ((2 * pi / 360) * EarthRadius) / 1000;        

        public static double DistanceToLat(double lat, double distance)
        {
            return lat + distance * meter1;
        }

        public static double DistanceToLon(double lon, double lat, double distance)
        {
            return lon + (distance * meter1) / Math.Cos(lat * (pi / 180));
        }

        public static double LatDistance(double latFrom, double latTo)
        {
            return (latFrom - latTo) * NauticalMile * 60;
        }

        public static double LonDistance(double lonFrom, double lonTo, double lat)
        {
            double latCorrection = Math.Cos(lat * (Math.PI / 180));
            return (lonFrom - lonTo) * NauticalMile * 60 * latCorrection;
        }

        public static double Distance(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return EarthRadius * 1000 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}