namespace ImFine.Client.ViewModels
{
    [QueryProperty(nameof(GroupName), nameof(GroupName))]
    public class GroupMemberViewModel : BaseViewModel
    {

        private string groupName = default;
        private string name = default;

        private string imageSource = "stop.svg";
        private string updatedAt;
        private string lastSeen = default;
        private string currentUser = default;

        public Command ViewMapCommand { get; }

        public GroupMemberViewModel()
        {
            ViewMapCommand = new Command(async () => await ViewOnMap());
        }
        public string LastSeen
        {
            get => $"Last Seen: {lastSeen}";
            set
            {
                SetProperty(ref lastSeen, value);
            }
        }
        public string UpdatedAt
        {
            get => $"Last Modified: {updatedAt}";
            set
            {
                SetProperty(ref updatedAt, value);
            }
        }

        public string ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, $"{value}.svg");
        }
        public string GroupName
        {
            get
            {
                return groupName;
            }
            set
            {
                groupName = value ?? throw new ArgumentNullException(nameof(value));
                GetGroup();
            }
        }

        public string CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                SetProperty(ref currentUser, value);
            }
        }

        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }

        public async void GetGroup()
        {
            IsBusy = true;
            var group = await GroupService.GetGroup(groupName);
            Name = group.name;
            ImageSource = group.status;
            UpdatedAt = group.updatedAt.ToLocalTime().ToString();
            LastSeen = group.lastSeen;
            currentUser = group.currentUser;
            IsBusy = false;
        }

        public async Task ViewOnMap()
        {
            try
            {
                string[] lastSeen = this.lastSeen.Split(",");
                var location = new Location(double.Parse(lastSeen[0]), double.Parse(lastSeen[1]));
                var options = new MapLaunchOptions { Name = $"{this.currentUser}'s last seen" };
                await Map.Default.OpenAsync(location, options);
            }
            catch (Exception _)
            {
                await Shell.Current.DisplayAlert("Error: ", "Unable to load map", "OK");
            }
        }
    }
}
