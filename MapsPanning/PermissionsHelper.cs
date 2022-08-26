using System;
using static Xamarin.Essentials.Permissions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using Newtonsoft.Json;

namespace MapsPanning
{
    public static class PermissionsHelper
    {
        public async static Task<PermissionStatus> CheckAndRequestPermissionFromTaskCompletionAsync<T>(T permission)
            where T : BasePermission
        {
            TaskCompletionSource<PermissionStatus> tcs = new TaskCompletionSource<PermissionStatus>();
            PermissionStatus status = await permission.CheckStatusAsync();
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (status != PermissionStatus.Granted)
                {
                    status = await permission.RequestAsync();
                    tcs.SetResult(status);
                }
                else
                {
                    tcs.SetResult(status);
                }
            });
            return await tcs.Task;
        }

        public static async Task<bool> HasPermission(BasePlatformPermission permission)
        {
            PermissionStatus status = await permission.CheckStatusAsync();
            return status == PermissionStatus.Granted;
        }

        private async static Task<PermissionStatus> GetLocationPermission()
        {
            return await CheckAndRequestPermissionFromTaskCompletionAsync(new LocationWhenInUse());
        }

        public static async Task<bool> CheckAndGetLocationPermission()
        {
            PermissionStatus status = await GetLocationPermission();
            if (status != PermissionStatus.Granted)
            {
                return false;
            }
            return true;
        }

        public async static Task<LocationModel> GetLastLocationAsync()
        {
            try
            {
                Xamarin.Essentials.PermissionStatus status = await CheckAndRequestPermissionFromTaskCompletionAsync(new Permissions.LocationWhenInUse());
                if (status != PermissionStatus.Granted)
                {
                    return null;
                }
                Xamarin.Essentials.Location location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        return JsonConvert.DeserializeObject<LocationModel>(JsonConvert.SerializeObject(placemark));
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine($"Exception {fnsEx}");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine($"Exception {fneEx}");
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine($"Exception {pEx}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex}");
            }
            return null;
        }

        public static async Task<LocationModel> GetLocationFromCache()
        {
            bool result = await HasPermission(new LocationWhenInUse());
            if (!result)
            {
                return null;
            }

            string locationString = Preferences.Get(App.UserLocation, string.Empty);
            if (!string.IsNullOrWhiteSpace(locationString))
            {
                try
                {
                    return JsonConvert.DeserializeObject<LocationModel>(locationString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception {ex}");
                }
            }
            else
            {
                await SetLocationInCache();
                locationString = Preferences.Get(App.UserLocation, string.Empty);
                if (!string.IsNullOrWhiteSpace(locationString))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<LocationModel>(locationString);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception {ex}");
                    }
                }
            }
            return null;
        }

        public static async Task SetLocationInCache()
        {
            LocationModel locationModel = await GetCurrentLocationAsync();
            if (locationModel == null)
            {
                locationModel = await GetLastLocationAsync();
            }

            if (locationModel != null)
            {
                SetLocationInCache(locationModel);
            }
        }

        public static void SetLocationInCache(LocationModel locationModel)
        {
            Preferences.Set(App.UserLocation, JsonConvert.SerializeObject(locationModel));
        }

        public async static Task<LocationModel> GetCurrentLocationAsync()
        {
            try
            {
                Xamarin.Essentials.PermissionStatus status = await CheckAndRequestPermissionFromTaskCompletionAsync(new Permissions.LocationWhenInUse());
                if (status != PermissionStatus.Granted)
                {
                    return null;
                }

                GeolocationRequest geolocationRequest = new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(15)
                };
                Xamarin.Essentials.Location location = await Geolocation.GetLocationAsync(geolocationRequest).ConfigureAwait(false);
                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();
                    if (placemark != null)
                    {
                        return JsonConvert.DeserializeObject<LocationModel>(JsonConvert.SerializeObject(placemark));
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine($"Exception {fnsEx}");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine($"Exception {fneEx}");
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine($"Exception {pEx}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex}");
            }
            return null;
        }
    }
}


