using Server.Models;

namespace Server.Repositories.IRepositories
{
    public interface IMessageRepository
    {
        Task<Message> AddMessage(Message msg);
        Task<List<Message>> GetMessagesBetweenTwoUsers(int userId1, int userId2);
        Task<List<Message>> GetGroupMessages(int groupId);
    }
}
