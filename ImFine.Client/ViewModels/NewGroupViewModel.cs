using ImFine.Client.DTOs;

namespace ImFine.Client.ViewModels
{
    public class NewGroupViewModel : BaseViewModel
    {
        private string name;
        private int intervalInMinutes;

        public NewGroupViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }
        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name) && intervalInMinutes != 0;
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public int IntervalInMinutes
        {
            get => intervalInMinutes;
            set => SetProperty(ref intervalInMinutes, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            GroupCreateDto newGroup = new()
            {
                name = Name,
                intervalInMinutes = IntervalInMinutes
            };

            try
            {
                var created = await GroupService.AddGroup(newGroup);
                if (created)
                {
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error: ", "Unable to create group", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error: ", $"{ex.Message}", "OK");
            }
        }
    }
}
