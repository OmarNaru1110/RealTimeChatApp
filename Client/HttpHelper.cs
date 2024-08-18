using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Client
{
    internal static class HttpHelper
    {
        public static async Task<bool> Register(HttpClient client, User user)
        {
            using StringContent jsonContent = new(JsonConvert.SerializeObject(user), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await client.PostAsync("api/Auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
        public static async Task<User> SignIn(HttpClient client, User user)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:5233/api/Auth/signin"),
                Content = new StringContent(
                    JsonConvert.SerializeObject(user),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json), // or "application/json" in older versions
            };

            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
                return null;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<User>(jsonResult);
            return userResult;
        }
        public static async Task<List<UsernameAndId>> SearchUsers(HttpClient client, string username)
        {

            using var response = await client.GetAsync($"api/user/search?username={username}");
            if (response.IsSuccessStatusCode == false)
                return null;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<List<UsernameAndId>>(jsonResult);
            return userResult;
        }
        public static async Task<List<UsernameAndId>> GetChats(HttpClient client, int userId)
        {

            using var response = await client.GetAsync($"api/user/chats?userId={userId}");
            if (response.IsSuccessStatusCode == false)
                return null;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<List<UsernameAndId>>(jsonResult);
            if (userResult == null)
                return null;
            foreach (var user in userResult)
            {
                await Console.Out.WriteLineAsync($"[{user.UserId}] {user.Username}");
            }
            return userResult;
        }
        public static async Task<List<Group>> GetGroups(HttpClient client, int userId)
        {

            using var response = await client.GetAsync($"api/user/groups?userId={userId}");
            if (response.IsSuccessStatusCode == false)
                return null;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var groupsResult = JsonConvert.DeserializeObject<List<Group>>(jsonResult);
            if (groupsResult == null)
                return null;
            foreach (var group in groupsResult)
            {
                await Console.Out.WriteLineAsync($"[{group.Id}] {group.Name}");
            }
            return groupsResult;
        }
        public static async Task<bool> AddGroup(HttpClient client, Group group)
        {
            using StringContent jsonContent = new(JsonConvert.SerializeObject(group), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await client.PostAsync("api/user/group", jsonContent);

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
        public static async Task<bool> UpdateGroup(HttpClient client, Group group)
        {
            using StringContent jsonContent = new(JsonConvert.SerializeObject(group), Encoding.UTF8, MediaTypeNames.Application.Json);
            using var response = await client.PutAsync("api/user/group", jsonContent);

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
