using ImFine.Server.Models;

namespace ImFine.Server.Contracts
{
    public interface ICosmosService
    {
        public Task CreateGroupAsync(Group group);
        public Task UpdateGroupAsync(Group group);
        public Task<Group?> GetGroupAsync(string name);
        public Task DeleteGroupAsync(Group group);
        public Task<IEnumerable<GroupReadDto>> GetGroupsByUsernameAndMembers(string username);
        public Task<IEnumerable<GroupSearchDto>> GetGroupsByGroupName(string searchTerm, string username);
    }
}
