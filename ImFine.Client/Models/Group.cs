namespace ImFine.Client.Models
{
    public class Group
    {
        public string id { get; set; }
        public string name { get; set; }
        public int intervalInMinutes { get; set; }
        public string status { get; set; }
        public string owner { get; set; }
        public string members { get; set; }
        public string currentUser { get; set; }
        public string lastSeen { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public Color BorderColor
        {
            get { return status != "stop" ? Colors.DarkGreen : Colors.DarkRed; }
        }

        public string Info
        {
            get { return $"Created by: {owner} \nCurrent User: {(string.IsNullOrEmpty(currentUser) ? "None" : currentUser)}"; }
        }

        //public async void OnItemTapped(object sender, TappedEventArgs args)
        //{
        //    var group = sender as Group;
        //    await Shell.Current.GoToAsync($"{nameof(GroupMemberPage)}?{nameof(GroupMemberViewModel.GroupName)}={group.name}");
        //}
    }
}
