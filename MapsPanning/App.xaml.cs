using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsPanning
{
    public partial class App : Application
    {
        public const string UserLocation = "user_location";

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
            SetLocation();
        }
        private void SetLocation()
        {
            Task.Run(async () => {
                await PermissionsHelper.SetLocationInCache();
            });
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
