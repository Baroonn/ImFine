using ImFine.Server.Contracts;
using ImFine.Server.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ImFine.Server.Hubs
{
    [Authorize]
    public class StatusHub : Hub<IStatusHubClient>
    {
        private readonly ICosmosService _cosmosService;
        private readonly string claim = "nickname";
        public StatusHub(ICosmosService service)
        {
            _cosmosService = service;
        }

        public async Task UpdateGroupStateWithoutNotification(string groupName, string state, string lastSeen, string message)
        {
            var user = Context.User?.FindFirst(c => c.Type == claim)?.Value;

            var group = await _cosmosService.GetGroupAsync(groupName);
            if (group == null)
            {
                throw new GroupNotFoundException(groupName);
            }
            if (!group.members.Contains($"{user}|") && group.owner != user)
            {
                throw new GroupUnauthorizedException();
            }
            //If group is currently being used and you are not the user
            if (group.status != "stop" && group.currentUser != user)
            {
                throw new GroupUnauthorizedException();
            }
            var oldStatus = group.status;
            group.status = state;
            group.updatedAt = DateTime.UtcNow;
            group.currentUser = state == "stop" ? "" : user;
            group.lastSeen = lastSeen;
            await _cosmosService.UpdateGroupAsync(group);
        }
        public async Task UpdateGroupState(string groupName, string state, string lastSeen, string message)
        {
            var user = Context.User?.FindFirst(c => c.Type == claim)?.Value;

            var group = await _cosmosService.GetGroupAsync(groupName);
            if (group == null)
            {
                throw new GroupNotFoundException(groupName);
            }
            if (!group.members.Contains($"{user}|") && group.owner != user)
            {
                throw new GroupUnauthorizedException();
            }
            //If group is currently being used and you are not the user
            if (group.status != "stop" && group.currentUser != user)
            {
                throw new GroupUnauthorizedException();
            }
            var oldStatus = group.status;
            group.status = state;
            group.updatedAt = DateTime.UtcNow;
            group.currentUser = state == "stop" ? "" : user;
            group.lastSeen = lastSeen.Contains("Latitude") ? lastSeen : group.lastSeen;
            await _cosmosService.UpdateGroupAsync(group);
            //await Clients.Caller.ReceiveMessage(groupName, state, message);
            if (oldStatus != state)
            {
                await Clients.OthersInGroup(groupName).ReceiveMessage(groupName, state, $"User {Context.User?.Claims.FirstOrDefault(u => u.Type == claim)?.Value} has changed the group state to {state}");
            }
        }

        public async Task AddUserToGroupWithoutNotify(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task AddUserToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).ReceiveMessage(groupName, "Joined", $"User {Context.User?.Claims.FirstOrDefault(u => u.Type == claim)?.Value} joined");
        }
        public async Task RemoveUserFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).ReceiveMessage(groupName, "Left", $"User {Context.User?.Claims.FirstOrDefault(u => u.Type == claim)?.Value} left");
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User?.Claims.FirstOrDefault(u => u.Type == claim)?.Value;
            var groups = _cosmosService.GetGroupsByUsernameAndMembers(user);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
