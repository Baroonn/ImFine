namespace ImFine.Client.ViewModels
{
    [QueryProperty(nameof(GroupName), nameof(GroupName))]
    public class GroupOwnerViewModel : BaseViewModel
    {
        //public string id { get; set; }
        private string groupName = default;
        private string name = default;
        private int intervalInMinutes;
        private string status = default;
        private bool? isToggled = null;
        private string imageSource = "stop.svg";
        private string updatedAt;
        private string lastSeen = default;


        public Command SafeCommand { get; }
        public Command UnsafeCommand { get; }
        public string UpdatedAt
        {
            get => $"Last Modified: {updatedAt}";
            set
            {
                SetProperty(ref updatedAt, value);
            }
        }



        public string LastSeen
        {
            get => $"Last Seen: {lastSeen}";
            set
            {
                SetProperty(ref lastSeen, value);
            }
        }
        public GroupOwnerViewModel()
        {
            SafeCommand = new Command(OnSafe, ValidateAction);
            UnsafeCommand = new Command(OnNotSafe, ValidateAction);

            this.PropertyChanged +=
                (_, args) =>
                {
                    if (args.PropertyName == nameof(Status)) { UpdateGroupState(); }
                };
        }

        public void Clear()
        {
            this.name = default;
            this.intervalInMinutes = default;
            this.status = default;
            this.isToggled = null;
            //ImageSource = "stopped.svg";
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
                Clear();
                GetGroup();
            }
        }

        public string ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, $"{value}.svg");
        }

        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }

        public int IntervalInMinutes
        {
            get => intervalInMinutes;
            set => SetProperty(ref intervalInMinutes, value);
        }
        public string Status
        {
            get => status;
            set
            {

                if (status == value || String.IsNullOrEmpty(status))
                    return;

                SetProperty(ref status, value);

            }
        }

        public bool IsToggled
        {
            get => isToggled ?? false;
            set
            {
                //ImageSource = value ? "ok.svg" : "stopped.svg";
                if (isToggled == value) return;
                //isToggled == null: Initial page load
                if (isToggled != null)
                {
                    Status = value ? "start" : "stop";
                }

                SetProperty(ref isToggled, value);
            }
        }

        public async void GetGroup()
        {
            IsBusy = true;
            var group = await GroupService.GetGroup(groupName);
            Name = group.name;
            IntervalInMinutes = group.intervalInMinutes;
            status = group.status;
            IsToggled = group.status != "stop";
            ImageSource = group.status;
            UpdatedAt = group.updatedAt.ToLocalTime().ToString();
            LastSeen = group.lastSeen;
            SafeCommand.ChangeCanExecute(); UnsafeCommand.ChangeCanExecute();
            IsBusy = false;
        }

        public async void UpdateGroupState()
        {
            IsBusy = true;
            string lastSeen = await GetCurrentLocation();
            await HubService.UpdateGroupStateAsync(groupName, status, lastSeen);
            if (status == "start")
            {
                AddToTimers(groupName, intervalInMinutes);
            }
            else if (status == "stop")
            {
                RemoveFromTimers(groupName);
            }
            if (status != "stop")
            {
#if ANDROID

                Android.App.Application.Context.StartForegroundService(Intent);
#endif
            }
            else
            {
                Android.App.Application.Context.StopService(Intent);
            }
            UpdatedAt = DateTime.Now.ToString();
            ImageSource = status;
            LastSeen = lastSeen;
            SafeCommand.ChangeCanExecute(); UnsafeCommand.ChangeCanExecute();
            IsBusy = false;
        }




        private bool ValidateAction()
        {
            return status != "stop";
        }

        public async void OnSafe()
        {
            IsBusy = true;
            string lastSeen = await GetCurrentLocation();
            await HubService.UpdateGroupStateAsync(groupName, "start", lastSeen);
            UpdatedAt = DateTime.Now.ToString();
            ImageSource = "start";
            LastSeen = lastSeen;
            SafeCommand.ChangeCanExecute(); UnsafeCommand.ChangeCanExecute();
            IsBusy = false;
        }

        public async void OnNotSafe()
        {
            IsBusy = true;
            string lastSeen = await GetCurrentLocation();
            await HubService.UpdateGroupStateAsync(groupName, "unsafe", lastSeen);
            UpdatedAt = DateTime.Now.ToString();
            ImageSource = "unsafe";
            LastSeen = lastSeen;
            SafeCommand.ChangeCanExecute(); UnsafeCommand.ChangeCanExecute();
            IsBusy = false;
        }


    }
}