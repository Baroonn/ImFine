#if ANDROID 
using ImFine.Client.Platforms.Android;
#endif
using ImFine.Client.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImFine.Client.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
#if ANDROID
        private readonly Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context, typeof(ForegroundServiceDemo));
#endif
        private static Dictionary<string, System.Timers.Timer> timers = new();

        public BaseViewModel()
        {
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }


        private async void Current_NotificationActionTapped(NotificationActionEventArgs e)
        {
            LocalNotificationCenter.Current.Cancel(e.Request.NotificationId);
            switch (e.ActionId)
            {
                case 100:
                    await HubService.UpdateGroupStateAsync(e.Request.Title, "start", await GetCurrentLocation());
                    break;

                case 101:
                    await HubService.UpdateGroupStateAsync(e.Request.Title, "unsafe", await GetCurrentLocation());
                    break;
            }

        }
        public void AddToTimers(string groupName, int intervalInMinutes)
        {
            Timers[groupName] = new System.Timers.Timer(intervalInMinutes * 60000);
            Timers[groupName].AutoReset = false;
            Timers[groupName].Elapsed += async (sender, e) =>
            {
                var group = await GroupService.GetGroup(groupName);
                if (DateTime.UtcNow > group.updatedAt.AddMinutes(intervalInMinutes * 1.5))
                {
                    var lastSeen = await GetCurrentLocation();
                    lastSeen = lastSeen == "None" ? await GetCachedLocation() : lastSeen;
                    await HubService.UpdateGroupStateAsync(groupName, "unsafe", lastSeen);
                }
                var request = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = groupName,
                    Subtitle = "Status",
                    Description = "Are you safe?",
                    BadgeNumber = 42,
                    CategoryType = NotificationCategoryType.Status,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                };
                await LocalNotificationCenter.Current.Show(request);
                if (Timers[groupName] != null) Timers[groupName].Enabled = true;
            };
            Timers[groupName].Start();
        }

        public void RemoveFromTimers(string groupName)
        {
            if (Timers.TryGetValue(groupName, out System.Timers.Timer value))
            {
                value.Stop();
                value.Dispose();
                Timers.Remove(groupName);
            }
        }
        public Dictionary<string, System.Timers.Timer> Timers => timers;
#if ANDROID
        public Android.Content.Intent Intent => intent;
#endif
        public static IGroupService GroupService => ServiceHelper.GetService<IGroupService>();
        public static IHubService HubService => ServiceHelper.GetService<IHubService>();
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this,
            new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public async Task<string> GetCachedLocation()
        {
            try
            {
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location != null)
                    return $"Latitude: {location.Latitude}, Longitude: {location.Longitude}";
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }

            return "None";
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            //if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            //{
            //    // Prompt the user with additional information as to why the permission is needed
            //}
            var accept = await Shell.Current.DisplayAlert("Info: ", "This app collects location data to enable you share you location with users in your groups even when the app is closed.", "OK", "Deny");

            if (!accept) return status;
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        public async Task<string> GetCurrentLocation()
        {
            Location location;
            try
            {
                _isCheckingLocation = true;
                var status = await CheckAndRequestLocationPermission();
                if (status != PermissionStatus.Granted)
                {
                    return "None";
                }
                GeolocationRequest request = new(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return $"{location.Latitude}, {location.Longitude}";
                }

            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;

            }
            return "None";
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
    }
}
