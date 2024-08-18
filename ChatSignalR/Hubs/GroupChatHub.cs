using Microsoft.AspNetCore.SignalR;
using Server.Dtos;
using Server.Models;
using Server.Repositories.IRepositories;

namespace Server.Hubs
{
    public class GroupChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupRepository _groupRepository;

        public GroupChatHub(IUserRepository userRepository, IMessageRepository messageRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _groupRepository = groupRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var groupIdStr = Context.GetHttpContext().Request.Query["groupId"].FirstOrDefault();//ahmed

            var groupParseResult = int.TryParse(groupIdStr, out int groupId);
            if (groupParseResult == false)
                return;

            var group = await _groupRepository.Get(groupId);
            if (group == null)
                return;

            group.GroupConnections.Add(new Models.GroupConnection { ConnectionId = Context.ConnectionId, GroupId = groupId});
            await _groupRepository.UpdateGroup(group);

            var messages = await _messageRepository.GetGroupMessages(groupId);
            foreach (var msg in messages)
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
            if (msg == null || msg.GroupId == null)
                return;

            await _messageRepository.AddMessage(new Message
            {
                CreatedDate = DateTime.Now,
                SenderId = msg.SenderId,
                Content = msg.Content,
                GroupId = msg.GroupId,
            });
            await SendMessageAsync(msg);
        }
        private async Task SendMessageAsync(MessageDto msg)
        {
            var connections = await _groupRepository.GetGroupConnections(msg.GroupId.Value);
            if (connections == null)
                return;
            foreach (var connectionId in connections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", msg);
            }

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var groupId = Context.GetHttpContext().Request.Query["groupId"].FirstOrDefault();
            if (groupId != null && int.TryParse(groupId, out var parsedGroupId))
                await _groupRepository.RemoveConnectionId(parsedGroupId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
