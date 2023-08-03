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
            IsBusy = false;
        }
    }
}
