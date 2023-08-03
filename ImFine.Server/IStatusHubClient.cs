namespace ImFine.Server
{
    public interface IStatusHubClient
    {
        Task ReceiveMessage(string group, string state, string message);
    }
}
