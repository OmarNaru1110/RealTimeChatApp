using Client.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using System.Data;
using System.Net.Http;
using System.Security.Principal;
namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5233")
            };

            while (true)
            {
                var user = new User();
                Helper.Print("Welcome To Chat");
                await Console.Out.WriteLineAsync();
                await Console.Out.WriteLineAsync("[1] register");
                await Console.Out.WriteLineAsync("[2] sign in");
                var choice = int.Parse(await Console.In.ReadLineAsync());
                switch (choice)
                {
                    case 1:
                    default:
                        //register logic
                        Helper.Clear();
                        await Console.Out.WriteLineAsync("UserName:");
                        user.Username = Console.ReadLine();
                        await Console.Out.WriteLineAsync("password:");
                        user.Password = Helper.InputPassword();
                        //calling register endpoint
                        var registerResult = await HttpHelper.Register(client, user);
                        if (registerResult == false)
                            Helper.Print("something went wrong! please register again");
                        else
                            Helper.Print("registered successfully! you can sign in now");
                        break;
                    case 2:
                        //login login
                        Helper.Clear();
                        await Console.Out.WriteLineAsync("UserName:");
                        user.Username = Console.ReadLine();
                        await Console.Out.WriteLineAsync("password:");
                        user.Password = Helper.InputPassword();
                        //calling sign in endpoint
                        user =  await HttpHelper.SignIn(client, user);
                        if (user == null)
                        {
                            Helper.Print("username or password is incorrect");
                            break;
                        }
                        Helper.Print($"{user.Username} signed in successfully");

                        while (true)
                        {
                            Helper.Clear();
                            await Console.Out.WriteLineAsync("[1] chats\n[2] groups\n[3] search for users\n[4] back");

                            bool back = false;
                            choice = int.Parse(Console.ReadLine());
                            Helper.Clear();

                            switch (choice)
                            {
                                case 1:
                                default:
                                    //chat logic
                                    var chats = await HttpHelper.GetChats(client, user.Id);
                                    if (chats == null || chats.Count == 0)
                                    { 
                                        await Console.Out.WriteLineAsync("no chats found");
                                        Console.ReadLine();
                                        break;
                                    }
                                    //chat with any user logic
                                    await Helper.BeforeChat(chats, user, client);
                                    break;
                                case 2:
                                    //group chat logic
                                    await Console.Out.WriteLineAsync("[0] Create Group");
                                    var groups = await HttpHelper.GetGroups(client, user.Id); 
                                    await Console.Out.WriteLineAsync("Enter 0 to create new group or groupid to enter a group");
                                    //list the groups here 
                                    choice = int.Parse(Console.ReadLine());
                                    Helper.Clear();
                                    if(choice == 0)
                                    {
                                        //create group logic
                                        await Helper.CreateGroup(client, user);
                                    }
                                    else
                                    {
                                        var group = groups.SingleOrDefault(g => g.Id == choice);
                                        Helper.Clear();
                                        if (group == null)
                                            Helper.Print("group not found");
                                        else
                                        {
                                            await Console.Out.WriteLineAsync($"{group.Name}\n---------------------------------------");
                                            await Console.Out.WriteLineAsync($"[1] Add members to group\n[2] Group chat");
                                            choice = int.Parse(Console.ReadLine());
                                            Helper.Clear();
                                            switch (choice)
                                            {
                                                case 1:
                                                    Helper.AddMembersToGroup(group);
                                                    await  HttpHelper.UpdateGroup(client, group);
                                                    break;
                                                case 2:
                                                default:
                                                    //group chat
                                                    await Helper.BeforeGroupChat(group, client, user);
                                                    break;
                                            }
                                        }
                                            
                                    }
                                    break;
                                case 3:
                                    //search for users logic
                                    var searchForUsers = await Helper.SearchForUsers(client);
                                    if(searchForUsers == null || searchForUsers.Count == 0)
                                    {
                                        await Console.Out.WriteLineAsync("no users found");
                                        Console.ReadLine();
                                        break;
                                    }
                                    //chat with any user logic
                                    await Helper.BeforeChat(searchForUsers, user, client);
                                    break;
                                case 4:
                                    back = true;
                                    break;
                            }
                            if (back)
                                break;
                        }
                        break;
                }
            }
        }
    }
}
