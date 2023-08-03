using ImFine.Client.Models;
using ImFine.Client.Views;
using System.Collections.ObjectModel;

namespace ImFine.Client.ViewModels
{
    public class GroupListViewModel : BaseViewModel
    {

        public ObservableCollection<Group>? Groups { get; set; } = new();

        public Command GetGroupsCommand { get; }
        public Command AddGroupCommand { get; }
        public Command FindGroupCommand { get; }
        public Command GoToGroupCommand { get; }
        public Command RefreshGroupsCommand { get; }
        public GroupListViewModel()
        {
            GetGroupsCommand = new Command(async () => await GetGroupsAsync(true));
            RefreshGroupsCommand = new Command(async () => await GetGroupsAsync(false));
            AddGroupCommand = new Command(AddGroupAsync);
            FindGroupCommand = new Command(FindGroupAsync);
            GoToGroupCommand = new Command<Group>(GoToGroup);

            GetGroupsCommand.Execute(this);
        }

        //notify is set to false if we are just refreshing to page (to prevent sending messages to other connected users).
        public async Task GetGroupsAsync(bool notify)
        {

            try
            {
                IsBusy = true;
                bool foreground = false;
                var groups = await GroupService.GetGroups();

                if (Groups.Count != 0)
                {
                    Groups.Clear();
                }

                foreach (var group in groups)
                {
                    await HubService.AddUserToGroupAsync(group.name, notify);
                    Groups.Add(group);
                    if (group.status != "stop" && group.currentUser == (await SecureStorage.GetAsync("username")))
                        AddToTimers(group.name, group.intervalInMinutes);
                    else RemoveFromTimers(group.name);
                    if (group.status != "stop")
                    {
                        foreground = true;
                    }
                }
                if (foreground)
                {
#if ANDROID

                    Android.App.Application.Context.StartForegroundService(Intent);
#endif
                }
                else
                {
                    Android.App.Application.Context.StopService(Intent);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error: ", "Unable to load groups", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void AddGroupAsync(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewGroupPage));
        }

        public async void FindGroupAsync(object obj)
        {
            await Shell.Current.GoToAsync(nameof(GroupSearchPage));
        }

        public async void GoToGroup(Group group)
        {
            var currentUser = await SecureStorage.GetAsync("username");
            if (group.status != "stop" && group.currentUser != currentUser)
            {
                await Shell.Current.GoToAsync($"{nameof(GroupMemberPage)}?{nameof(GroupMemberViewModel.GroupName)}={group.name}");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(GroupOwnerPage)}?{nameof(GroupOwnerViewModel.GroupName)}={group.name}");
            }
        }
    }
}
