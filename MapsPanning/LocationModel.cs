using System;
using Xamarin.Essentials;

namespace MapsPanning
{
    public class LocationModel
    {
        public Location Location { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string FeatureName { get; set; }

        public string PostalCode { get; set; }

        public string SubLocality { get; set; }

        public string Thoroughfare { get; set; }

        public string SubThoroughfare { get; set; }

        public string Locality { get; set; }

        public string AdminArea { get; set; }

        public string SubAdminArea { get; set; }
    }
}

