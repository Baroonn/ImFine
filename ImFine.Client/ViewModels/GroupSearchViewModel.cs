using ImFine.Client.DTOs;
using ImFine.Client.Models.Exceptions;
using ImFine.Client.Views;
using System.Collections.ObjectModel;

namespace ImFine.Client.ViewModels
{
    public class GroupSearchViewModel : BaseViewModel
    {
        public ObservableCollection<GroupSearchDto> Groups { get; set; } = new();
        private string searchTerm;
        public Command SearchCommand { get; }
        public Command JoinCommand { get; }
        public Command LeaveCommand { get; }
        public Command LoadCommand { get; }
        public GroupSearchViewModel()
        {
            SearchCommand = new Command(OnSearch, ValidateSave);
            JoinCommand = new Command<GroupSearchDto>(JoinGroup);
            LeaveCommand = new Command<GroupSearchDto>(LeaveGroup);
            LoadCommand = new Command(OnLoad);
            this.PropertyChanged +=
                (_, __) => SearchCommand.ChangeCanExecute();
        }

        private async void JoinGroup(GroupSearchDto group)
        {
            try
            {
                IsBusy = true;
                bool success;
                if (group.following)
                {
                    success = await GroupService.LeaveGroup(group.name);
                }
                else
                {
                    success = await GroupService.JoinGroup(group.name);
                }
                if (success)
                {
                    group.following = !group.following;
                    LoadCommand.Execute(this);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error: ", $"Unable to perform action", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error: ", $"{ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LeaveGroup(GroupSearchDto group)
        {
            await GroupService.LeaveGroup(group.name);
        }

        public string SearchTerm
        {
            get => searchTerm;
            set => SetProperty(ref searchTerm, value);
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(searchTerm);
        }

        public async void OnLoad()
        {
            try
            {
                IsBusy = true;
                var offlineGroups = Groups.ToList();

                if (Groups.Count != 0)
                {
                    Groups.Clear();
                }

                foreach (var group in offlineGroups)
                {
                    Groups.Add(group);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error: ", $"{ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async void OnSearch()
        {

            try
            {
                IsBusy = true;
                var groups = await GroupService.FindGroup(SearchTerm);

                if (Groups.Count != 0)
                {
                    Groups.Clear();
                }

                foreach (var group in groups)
                {
                    Groups.Add(group);
                }

            }
            catch (UserUnauthorizedException ex)
            {
                _ = Shell.Current.GoToAsync(nameof(LoginPage));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error: ", $"{ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
