using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Context;
using Server.Repositories.IRepositories;

namespace Server.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationContext _context;

        public MessageRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Message> AddMessage(Message msg)
        {
            await _context.Messages.AddAsync(msg);
            await _context.SaveChangesAsync();
            return msg;
        }

        public async Task<List<Message>> GetGroupMessages(int groupId)
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .Where(m=>m.GroupId == groupId);
            return messages.ToList();
        }

        public async Task<List<Message>> GetMessagesBetweenTwoUsers(int userId1, int userId2)
        {
            var messages = _context.Messages
                .Include(m=>m.Sender)
                .Where(m=>(m.SenderId == userId1 && m.ReceiverId == userId2) || (m.SenderId == userId2 && m.ReceiverId == userId1));
            return messages.ToList();
        }
    }
}
