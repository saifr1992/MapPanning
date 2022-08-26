using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MapsPanning.Model;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace MapsPanning
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Private Properties
        private Location currentLocation = null;
        #endregion

        #region Protected Properties

        #endregion

        #region Public Properties
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Command

        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            GetLocation();
        }
        #endregion

        #region Get Set
        public ObservableCollection<LocationHistory> _locationHistories = new ObservableCollection<LocationHistory>();
        public ObservableCollection<LocationHistory> LocationHistories
        {
            set
            {

                _locationHistories = value;
                OnPropertyChanged(nameof(LocationHistories));
            }
            get
            {
                return _locationHistories;
            }
        }

        public double _latitude;
        public double Latitude
        {
            set
            {

                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
            get
            {
                return _latitude;
            }
        }

        public double _longitude;
        public double Longitude
        {
            set
            {

                _longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
            get
            {
                return _longitude;
            }
        }
        #endregion

        #region Private Implementation
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetLocation()
        {
            _ = Task.Run(() => {
                Xamarin.Forms.Device.StartTimer(new System.TimeSpan(0, 0, 0, 5), () =>
                {
                    GetCurrentLocation();
                    GetLocationFromLocal();
                    return true;
                });
            });
        }

        private void GetCurrentLocation()
        {
            Task.Run(async () => {
                await PermissionsHelper.SetLocationInCache();
            });
        }

        private void GetLocationFromLocal()
        {
            LocationModel locationModel = null;
            string locationString = Preferences.Get(App.UserLocation, string.Empty);
            if (!string.IsNullOrWhiteSpace(locationString))
            {
                locationModel = JsonConvert.DeserializeObject<LocationModel>(locationString);
            }

            if (locationModel?.Location == null)
            {
                return;
            }

            Longitude = locationModel.Location.Longitude;
            Latitude = locationModel.Location.Latitude;

            // location is original
            Console.WriteLine($"Latitude: {Latitude}, Longitude: {Longitude}, Altitude: {locationModel.Location.Altitude}");

            if (currentLocation == null)
            {
                currentLocation = locationModel.Location;
                //setting location first time
                SetupCurrentLocation();
            }
            else
            {
                double distance = GetDistanceBetweenTwoLocations(locationModel.Location, currentLocation);
                if (distance >= 20)
                {
                    MainPage.DisplayMessageAction.Invoke(distance);
                    //setting up location after as current one
                    SetupCurrentLocation();
                }
            }
        }

        private double GetDistanceBetweenTwoLocations(Location current, Location previous)
        {
            Distance distance = Distance.BetweenPositions(new Position(current.Latitude, current.Longitude), new Position(previous.Latitude, previous.Longitude));
            return distance.Meters;
        }

        private async void SetupCurrentLocation()
        {
            MainPage.SetMapPositionAction?.Invoke(currentLocation);
            var placemarks = await Geocoding.GetPlacemarksAsync(currentLocation.Latitude, currentLocation.Longitude);
            var placemark = placemarks?.FirstOrDefault();
            string address = String.Empty;
            if (placemark != null)
            {
                address = $"{placemark.Thoroughfare}, {placemark.SubThoroughfare}, {placemark.SubAdminArea}, {placemark.AdminArea}, {placemark.PostalCode} ,  {placemark.CountryName}";
            }
            ObservableCollection<LocationHistory> locationHistories = new ObservableCollection<LocationHistory>();
            locationHistories.Add(new LocationHistory() { Position = new Position(currentLocation.Latitude, currentLocation.Longitude), Address = address });
            LocationHistories = locationHistories;

        }
        #endregion

        #region Protected Implementation

        #endregion

        #region Public Implementation

        #endregion





    }
}
