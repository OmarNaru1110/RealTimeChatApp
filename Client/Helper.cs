using Client.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal static class Helper
    {
        public static string InputPassword()
        {
            string password = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }
            return password;
        }
        public static void Print(string text)
        {
            Console.Clear();
            foreach (var c in text)
            {
                Console.Write(c);
                Thread.Sleep(50);
            }
            Thread.Sleep(1000);
        }

        public static async Task<List<UsernameAndId>> SearchForUsers(HttpClient client)
        {
            Console.WriteLine("Search For Users");
            Console.Write("search: ");
            var username = Console.ReadLine();
            var users = await HttpHelper.SearchUsers(client, username);
            if (users == null || users.Count == 0)
                return null;
            foreach (var user in users)
            {
                await Console.Out.WriteLineAsync($"[{user.UserId}] {user.Username}");
            }
            return users;
        }
        public static void Clear()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.Clear();
        }
        public static async Task BeforeChat(List<UsernameAndId> chats, User user, HttpClient client)
        {
            await Console.Out.WriteLineAsync("Enter user id to chat with: ");
            var receiverId = int.Parse(Console.ReadLine());
            Helper.Clear();
            var chatName = $"{chats.Single(u => u.UserId == receiverId).Username} Chat\n---------------------------------------";
            var connection = await Chat.InitializeConnection(user.Id, receiverId, chatName);
            var msg = new Message { GroupId = null, SenderId = user.Id, ReceiverId = receiverId, SenderUsername = user.Username };
            await Chat.SendMessages(connection, msg);
            Helper.Clear();
            await connection.DisposeAsync();
        }
        public static async Task CreateGroup(HttpClient client, User user)
        {
            var group = new Group();
            Console.WriteLine("group name: ");
            group.Name = Console.ReadLine();
            group.Members.Add(new UsernameAndId { Username = user.Username });
            Console.WriteLine("Members");
            group = AddMembersToGroup(group);
            if (await HttpHelper.AddGroup(client, group) == false)
                return;
            Print("group created successfully");
        }
        public static Group AddMembersToGroup(Group group)
        {
            Clear();
            Console.WriteLine($"{group.Name}\n---------------------------------------");
            var cnt = group.Members.Count;
            while (true)
            {
                Console.WriteLine("Enter member username to add to your group or empty to return: ");
                var username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                    break;
                group.Members.Add(new UsernameAndId { Username = username });
            }
            if (group.Members.Count > cnt)
                Print("Members Added Successfully");
            return group;
        }
        public static async Task BeforeGroupChat(Group group, HttpClient client, User user)
        {
            Clear();
            await Console.Out.WriteLineAsync($"{group.Name}\n---------------------------------------");
            var connection = await GroupChat.InitializeConnection(group);
            var msg = new Message { GroupId = group.Id, SenderId = user.Id, ReceiverId = null, SenderUsername = user.Username };
            await Chat.SendMessages(connection, msg);
            Clear();
            await connection.DisposeAsync();
        }
    }
}
