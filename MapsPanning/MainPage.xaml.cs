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
        public static Action<double> DisplayMessageAction;
        public static Action<Location> SetMapPositionAction;

        public MainPage()
        {
            InitializeComponent();
            DisplayMessageAction = DisplayMessage;
            SetMapPositionAction = SetMapPosition;
        }

        private void DisplayMessage(double distance)
        {
            DisplayAlert("Move Alert", $"You have moved {(int)distance} meters from your previous position", "Ok");
        }

        private void SetMapPosition(Location location)
        {
            if (location == null)
            {
                return;
            }
            double zoomLevel = 17;
            double latlongDegrees = 360 / (Math.Pow(2, zoomLevel));
            Position position = new Position(location.Latitude, location.Longitude); //User Actual Latitude and Longitude
            positionMap.MoveToRegion(new MapSpan(position, latlongDegrees, latlongDegrees));

        }
    }
}
