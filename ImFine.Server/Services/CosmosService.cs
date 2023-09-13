using ImFine.Server.Contracts;
using ImFine.Server.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ImFine.Server.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;
        private readonly IConfiguration _configuration;
        private Container container
        {
            get => _client
                .GetDatabase(_configuration["COSMOS:Database"])
                .GetContainer(_configuration["COSMOS:Container"]);
        }
        public CosmosService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new CosmosClient(
                connectionString: configuration["COSMOS:ConnectionString"]
            );
        }

        private string CleanStatus(string status)
        {
            if (status == "stop")
            {
                return "stopped";
            }
            else if (status == "start")
            {
                return "started";
            }
            else
            {
                return status;
            }
        }
        public async Task CreateGroupAsync(Group group)
        {
            await container.CreateItemAsync<Group>(group, new PartitionKey(group.owner));
        }

        public async Task UpdateGroupAsync(Group group)
        {
            await container.UpsertItemAsync<Group>(group, new PartitionKey(group.owner));
        }

        public async Task<Group?> GetGroupAsync(string name)
        {
            var queryable = container.GetItemLinqQueryable<Group>();
            using FeedIterator<Group> groups = queryable.Where(p => p.name == name).ToFeedIterator();
            Group? first = (await groups.ReadNextAsync())?.FirstOrDefault();
            return first;
        }

        public async Task DeleteGroupAsync(Group group)
        {
            await container.DeleteItemAsync<Group>(group.id, new PartitionKey(group.owner));
        }

        public async Task<IEnumerable<GroupReadDto>> GetGroupsByUsernameAndMembers(string username)
        {
            var queryable = container.GetItemLinqQueryable<GroupReadDto>();
            using FeedIterator<GroupReadDto> groups = queryable.Where(p => p.owner == username || p.members.Contains($"{username}|")).ToFeedIterator();
            List<GroupReadDto> groupReadDtos = new List<GroupReadDto>();
            while (groups.HasMoreResults)
            {
                FeedResponse<GroupReadDto> response = await groups.ReadNextAsync();
                foreach (var item in response)
                {
                    groupReadDtos.Add(item);
                }
            }

            return groupReadDtos;
        }

        public async Task<IEnumerable<GroupSearchDto>> GetGroupsByGroupName(string searchTerm, string username)
        {
            var queryable = container.GetItemLinqQueryable<Group>();
            using FeedIterator<Group> groups = queryable.Where(p => p.name.Contains(searchTerm)).ToFeedIterator();
            List<GroupSearchDto> groupReadDtos = new List<GroupSearchDto>();
            while (groups.HasMoreResults)
            {
                FeedResponse<Group> response = await groups.ReadNextAsync();
                foreach (var item in response)
                {
                    var newItem = new GroupSearchDto();
                    newItem.name = item.name;
                    newItem.intervalInMinutes = item.intervalInMinutes;
                    newItem.owner = item.owner;
                    newItem.following = item.owner == username || item.members.Contains($"{username}|");
                    groupReadDtos.Add(newItem);
                }
            }

            return groupReadDtos;
        }
    }
}
