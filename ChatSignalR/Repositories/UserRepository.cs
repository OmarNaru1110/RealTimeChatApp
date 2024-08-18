using Microsoft.EntityFrameworkCore;
using Server.Dtos;
using Server.Models;
using Server.Repositories.Context;
using Server.Repositories.IRepositories;

namespace Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<User> Get(int userId)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Id == userId); 
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<UsernameAndIdDto>> GetChats(int userId)
        {
            var receivers = await _context.Messages
                .Include(m=>m.Receiver)
                .Where(m => m.SenderId == userId).Select(m => m.Receiver).Distinct().ToListAsync();
            var receiversDto = new List<UsernameAndIdDto>();
            foreach (var receiver in receivers)
            {
                if(receiver != null)
                    receiversDto.Add(new UsernameAndIdDto { UserId = receiver.Id, Username = receiver.Username });
            }
            return receiversDto;
        }

        public async Task<List<string>> GetConnectionIdsByUserIdAndReceiverId(int receiverId, int userId)
        {
            var user = await Get(receiverId);
            if (user == null)
                return null;
            return user.Connections.Where(c=>c.ReceiverId==userId).Select(c=>c.ConnectionId).ToList();
        }

        public async Task RemoveConnectionId(int userId, string connectionId)
        {
            var user = await Get(userId);
            if(user == null) 
                return;
            var connection = user.Connections.SingleOrDefault(c => c.ConnectionId == connectionId);
            if(connection==null) 
                return;
            user.Connections.Remove(connection);
            await UpdateUser(user);
        }

        public async Task<List<UsernameAndIdDto>> Search(string username)
        {
            var users = _context.Users
                .Where(u => u.Username.Contains(username)).Select(u => new UsernameAndIdDto { UserId = u.Id, Username = u.Username });
            return users.ToList();
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
