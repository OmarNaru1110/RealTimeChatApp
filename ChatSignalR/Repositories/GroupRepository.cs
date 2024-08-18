using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Dtos;
using Server.Repositories.Context;
using Server.Repositories.IRepositories;
using System.Text.RegularExpressions;

namespace Server.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationContext _context;
        private readonly IUserRepository _userRepository;

        public GroupRepository(ApplicationContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }
        public async Task<GroupDto> GetGroup(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                return null;
            return new GroupDto
            {
                Id = groupId,
                Name = group.Name,
                Members = group.Members.Select(g =>
                new UsernameAndIdDto { UserId = g.Id, Username = g.Username }).ToList()
            };

        }

        public async Task<List<MessageDto>> GetGroupMessages(int groupId)
        {
            return (await _context.Groups
                .Include(g => g.Messages)
                .ThenInclude(m => m.Sender)
                .SingleOrDefaultAsync(g => g.Id == groupId))?.Messages.Select(m => new MessageDto
                {
                    Content = m.Content,
                    GroupId = m.GroupId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderUsername = m.Sender.Username
                }).ToList();
        }

        public async Task<List<GroupDto>> GetUserGroups(int userId)
        {

            var groups = (await _context.Users
                            .Include(u => u.Groups)
                            .SingleOrDefaultAsync(u => u.Id == userId))?.Groups;

            if (groups == null)
                return null;

            return new List<GroupDto>(
                    groups.Select(g => new GroupDto { Id = g.Id, Name = g.Name })
                );
        }
        public async Task AddGroup(GroupDto dto)
        {
            var group = new Models.Group
            {
                Name = dto.Name,
            };
            HashSet<string> usernames = new(dto.Members.Select(m=>m.Username));

            foreach (var username in usernames)
            {
                var user = await _userRepository.GetByUsername(username);
                if (user == null)
                    continue;
                group.Members.Add(user);
            }
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateGroup(GroupDto dto)
        {
            var group = await  _context.Groups.SingleOrDefaultAsync(g => g.Id == dto.Id);
            if (group == null) 
                return;

            foreach (var member in dto.Members)
            {
                var user = await _userRepository.GetByUsername(member.Username);
                if (user == null)
                    continue;
                group.Members.Add(user);
            }

            await UpdateGroup(group);
        }

        public async Task<Models.Group> Get(int groupId)
        {
            return await _context.Groups.SingleOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task UpdateGroup(Models.Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveConnectionId(int groupId, string connectionId)
        {
            var group = await Get(groupId);
            if (group == null) 
                return;
            var connection = group.GroupConnections.SingleOrDefault(c=>c.ConnectionId == connectionId);
            if (connection == null)
                return;
            group.GroupConnections.Remove(connection);
            await UpdateGroup(group);
        }

        public async Task<List<string>?> GetGroupConnections(int groupId)
        {
            var connections = (await _context.Groups
                .Include(g => g.GroupConnections.Where(c => c.GroupId == groupId))
                .SingleOrDefaultAsync(g => g.Id == groupId))?.GroupConnections.Select(c=>c.ConnectionId);
            if (connections == null)
                return null;
            return connections.ToList();
        }
    }
}
