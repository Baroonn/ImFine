namespace ImFine.Client.Services
{
    public interface IHubService
    {
        public Task UpdateGroupStateAsync(string groupName, string state, string lastSeen);
        public Task AddUserToGroupAsync(string groupName, bool notify);
    }
}
