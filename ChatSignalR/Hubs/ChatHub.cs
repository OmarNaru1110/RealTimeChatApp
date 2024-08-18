using Microsoft.AspNetCore.SignalR;
using Server.Dtos;
using Server.Models;
using Server.Repositories.IRepositories;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ChatHub(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }
        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();//ahmed
            var receiverIdStr = Context.GetHttpContext().Request.Query["receiverId"].FirstOrDefault();//naru

            var userParseResult = int.TryParse(userIdStr, out int userId);
            var receiverParseResult = int.TryParse(receiverIdStr, out int receiverId);
            if (userParseResult == false || receiverParseResult == false)
                return;

            var user = await _userRepository.Get(userId);
            var receiver = await _userRepository.Get(receiverId);
            if (user == null || receiver == null)
                return;

            user.Connections.Add(new Connection { ConnectionId = Context.ConnectionId, ReceiverId = receiverId });
            await _userRepository.UpdateUser(user);

            var messages = await _messageRepository.GetMessagesBetweenTwoUsers(userId, receiverId);
            foreach(var msg in messages)
            {
                var dto = new MessageDto
                {
                    Content = msg.Content,
                    SenderId = msg.SenderId,
                    SenderUsername = msg.Sender.Username,
                    ReceiverId = msg.ReceiverId,
                };
                await Clients.Caller.SendAsync("ReceiveMessage", dto);
            }

            await base.OnConnectedAsync();
        }
        public async Task SendMessage(MessageDto msg)
        {
            if (msg == null || msg.ReceiverId == null)
                return;

            await _messageRepository.AddMessage(new Message
            {
                CreatedDate = DateTime.Now,
                SenderId = msg.SenderId,
                Content = msg.Content,
                ReceiverId = msg.ReceiverId,
            });
            await SendMessageAsync(msg);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();
            if (userId != null && int.TryParse(userId, out var parsedUserId))
                await _userRepository.RemoveConnectionId(parsedUserId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendMessageAsync(MessageDto msg)
        {
            var connectionsReceiver = await _userRepository.GetConnectionIdsByUserIdAndReceiverId(msg.ReceiverId.Value, msg.SenderId);

            foreach (var connectionId in connectionsReceiver)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", msg);
            }

            if (msg.SenderId != msg.ReceiverId)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", msg);
            }

        }
    }
}
