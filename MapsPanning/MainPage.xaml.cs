using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MapsPanning
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LocationModel firstLocationModel = GetLocationFromLocal();
            SetMapPosition(firstLocationModel);
            _ = Task.Run(() => {
                Xamarin.Forms.Device.StartTimer(new System.TimeSpan(0, 0, 0, 10), () =>
                {
                    LocationModel locationModel = GetLocationFromLocal();
                    SetMapPosition(locationModel);
                    return true;
                });
            });
        }

        private LocationModel GetLocationFromLocal()
        {
            string locationString = Preferences.Get(App.UserLocation, string.Empty);
            if (!string.IsNullOrWhiteSpace(locationString))
            {
                LocationModel locationModel = JsonConvert.DeserializeObject<LocationModel>(locationString);
                return locationModel;
            }
            return null;
        }

        private void SetMapPosition(LocationModel locationModel)
        {
            if (locationModel?.Location == null)
            {
                return;
            }
            double zoomLevel = 17;
            double latlongDegrees = 360 / (Math.Pow(2, zoomLevel));
            Position position = new Position(locationModel.Location.Latitude, locationModel.Location.Longitude); //User Actual Latitude and Longitude
            positionMap.MoveToRegion(new MapSpan(position, latlongDegrees, latlongDegrees));

        }


        bool firsttime = true;
        void  positionMap_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e!= null)
            {
               
            }
            var m = (Xamarin.Forms.Maps.Map)sender;
            LocationModel getLocationFromCache = PermissionsHelper.GetLocationFromCache().Result;
            if (m.VisibleRegion != null)
            {
                if (firsttime == true)
                {
                    firsttime = false;
                    return;
                }
                Position position = new Position(m.VisibleRegion.Center.Latitude, m.VisibleRegion.Center.Longitude); //User Actual Latitude and Longitude
                GetLocation(position);

                if (getLocationFromCache != null)
                {
                    Position position2 = new Position(getLocationFromCache.Location.Latitude, getLocationFromCache.Location.Longitude);
                    double distancInMeter = CalculateDistance(position, position2);
                    UserLatitude.Text = m.VisibleRegion.Center.Latitude.ToString();
                    UserLongitude.Text = m.VisibleRegion.Center.Longitude.ToString();
                    if (distancInMeter > 20)
                    {
                        PermissionsHelper.SetOnlyLatiAndLongInCache(m.VisibleRegion.Center.Latitude, m.VisibleRegion.Center.Longitude);
                        DisplayAlert("Alert", $"You are away {distancInMeter} from your last location", "OK");
                    }
                    DistanceVal.Text = distancInMeter.ToString() + " meter";
                }
                

            }
        }

        public async void GetLocation(Position position)
        {
            try
            {
                var locationInfo = $"Latitude: {position.Latitude}\n" +
                $"Longitude: {position.Longitude}\n";

                

                var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    var geocodeAddress = "\n" +
                        $"{placemark.Thoroughfare}\n" + //Address
                        $"{placemark.SubLocality}\n" + //Address area name
                        $"{placemark.Locality} {placemark.SubAdminArea}\n" + //CityName
                        $"{placemark.PostalCode}\n" + //PostalCode
                        $"{placemark.AdminArea}\n" + //StateName
                        $"{placemark.CountryName}\n" + //CountryName
                        $"CountryCode: {placemark.CountryCode}\n";


                         mapInfo.Text = "";
                         mapInfo.Text += geocodeAddress;
                }
                else
                    await DisplayAlert("Error occurred", "Unable to retreive address information", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error occurred", ex.Message.ToString(), "Ok");
            }
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double CalculateDistance(Position location1, Position location2)
        {
            if(location1.Latitude== location2.Latitude && location1.Longitude == location2.Longitude)
            {
                return 0;
            }
           
            double distance = 0.0;
            var location = new Location(location1.Latitude, location1.Longitude);
            var otherLocation = new Location(location2.Latitude, location2.Longitude);
            distance = location.CalculateDistance(otherLocation, DistanceUnits.Kilometers);

            double meters = (double)Math.Round(1000 * distance, 0);
            return meters;
        }


    }
}
