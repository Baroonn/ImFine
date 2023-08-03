using IdentityModel.Client;
using ImFine.Client.DTOs;
using ImFine.Client.Models;
using ImFine.Client.Models.Exceptions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ImFine.Client.Services
{
    public class GroupService : IGroupService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public GroupService()
        {
            httpClient = new HttpClient();
            baseUrl = "https://imfine.azurewebsites.net";

        }


        public async Task<List<Group>> GetGroups()
        {
            List<Group> groups = new List<Group>();
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));

            var response = await httpClient.GetAsync($"{baseUrl}/api/groups");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.RemoveAll();
                throw new UserUnauthorizedException();
            }
            if (response.IsSuccessStatusCode)
            {
                groups = await response.Content.ReadFromJsonAsync<List<Group>>();
            }
            return groups;
        }

        public async Task<Group> GetGroup(string name)
        {
            Group group = null;
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));
            var response = await httpClient.GetAsync($"{baseUrl}/api/groups/{name}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.RemoveAll();
                throw new UserUnauthorizedException();
            }
            if (response.IsSuccessStatusCode)
            {
                group = await response.Content.ReadFromJsonAsync<Group>();
            }

            return group;
        }

        public async Task<bool> AddGroup(GroupCreateDto group)
        {
            var json = JsonConvert.SerializeObject(group);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));
            var response = await httpClient.PostAsync($"{baseUrl}/api/groups", data);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<GroupSearchDto>> FindGroup(string searchTerm)
        {
            List<GroupSearchDto> groups = new List<GroupSearchDto>();
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));
            var response = await httpClient.GetAsync($"{baseUrl}/api/groups/search?searchTerm={searchTerm}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.RemoveAll();
                throw new UserUnauthorizedException();
            }
            if (response.IsSuccessStatusCode)
            {
                groups = await response.Content.ReadFromJsonAsync<List<GroupSearchDto>>();
            }
            return groups;
        }

        public async Task<bool> JoinGroup(string groupName)
        {
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));
            var response = await httpClient.GetAsync($"{baseUrl}/api/groups/{groupName}/join");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UserUnauthorizedException();
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LeaveGroup(string groupName)
        {
            httpClient.SetBearerToken(await SecureStorage.Default.GetAsync("identity"));
            var response = await httpClient.GetAsync($"{baseUrl}/api/groups/{groupName}/leave");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UserUnauthorizedException();
            }
            return response.IsSuccessStatusCode;
        }
    }
}
