using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.SignalR
{
    internal class GroupChat
    {
        public static async Task<HubConnection> InitializeConnection(Group group)
        {
            var connection = new HubConnectionBuilder()
                                        .WithUrl($"https://localhost:7071/groupchathub?groupId={group.Id}")
                                        .Build();
            Helper.Clear();
            await Console.Out.WriteLineAsync($"{group.Name}\n---------------------------------------");
            await ReceiveMessage(connection, null);

            await connection.StartAsync();

            return connection;
        }
        private static async Task ReceiveMessage(HubConnection connection, Message? msg)
        {
            connection.Remove("ReceiveMessage");

            connection.On<Message>("ReceiveMessage", (result) =>
            {
                Console.WriteLine($"[{result.SenderUsername}] {result.Content}");
            });
        }
    }
}
