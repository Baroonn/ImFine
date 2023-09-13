using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;

namespace ImFine.Client.Services
{
    public class HubService : IHubService
    {
        private readonly HubConnection hubConnection;

        public HubService()
        {
            string accessToken = SecureStorage.Default.GetAsync("identity").Result;
            if (accessToken != null)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl($"https://imfine.azurewebsites.net/status?access_token={accessToken}")
                    .Build();
                hubConnection.HandshakeTimeout = new TimeSpan(8, 0, 0);
                hubConnection.On<string, string, string>("ReceiveMessage", (groupName, status, message) =>
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = 1000,
                        Title = $"{groupName}",
                        Subtitle = $"{status}",
                        Description = $"{message}",
                        BadgeNumber = 42,
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(1),
                        },
                    };

                    LocalNotificationCenter.Current.Show(request);
                });



                //Task.Run(() =>
                //{
                //    Application.Current.Dispatcher.Dispatch(async () => await hubConnection.StartAsync());
                //});
            }
        }


        public async Task StartConnectionAsync()
        {
            if (hubConnection.State != HubConnectionState.Connected)
            {
                await hubConnection.StartAsync();
            }
        }

        public async Task UpdateGroupStateAsync(string groupName, string state, string lastSeen)
        {
            try
            {
                await StartConnectionAsync();
                await hubConnection.InvokeCoreAsync("UpdateGroupState", args: new[] { groupName, state, lastSeen, "" });
            }
            catch (Exception _)
            {
                await Shell.Current.DisplayAlert("Error: ", "Unable to make changes to group. Confirm you have permission.", "OK");
            }
        }

        public async Task AddUserToGroupAsync(string groupName, bool notify)
        {
            await StartConnectionAsync();
            await hubConnection.InvokeCoreAsync(notify ? "AddUserToGroup" : "AddUserToGroupWithoutNotify", args: new[] { groupName });
        }
    }
}
