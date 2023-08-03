using ImFine.Client.DTOs;
using ImFine.Client.Models;

namespace ImFine.Client.Services
{
    public interface IGroupService
    {
        public Task<List<Group>> GetGroups();
        public Task<Group> GetGroup(string name);
        public Task<bool> AddGroup(GroupCreateDto group);
        public Task<List<GroupSearchDto>> FindGroup(string searchTerm);

        public Task<bool> JoinGroup(string groupName);
        public Task<bool> LeaveGroup(string groupName);
    }
}
