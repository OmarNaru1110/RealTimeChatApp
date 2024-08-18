using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.SignalR
{
    public static class Chat
    {
        public static async Task<HubConnection> InitializeConnection(int userId, int receiverId, string chatName)
        {
            var connection = new HubConnectionBuilder()
                                        .WithUrl($"https://localhost:7071/chathub?userId={userId}&receiverId={receiverId}")
                                        .Build();
            Helper.Clear();
            await Console.Out.WriteLineAsync(chatName);
            await ReceiveMessage(connection, null);

            await connection.StartAsync();

            return connection;
        }
        public async static Task SendMessages(HubConnection connection, Message msg)
        {
            try
            {
                while (true)
                {
                    if(connection?.State == HubConnectionState.Connected)
                    {
                        msg.Content = Console.ReadLine();
                        if (string.IsNullOrEmpty(msg.Content))
                            break;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write(new String(' ', Console.BufferWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        await connection.InvokeAsync("SendMessage", msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
            }
            return;
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
