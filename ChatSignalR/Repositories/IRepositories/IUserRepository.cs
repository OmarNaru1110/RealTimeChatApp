using Server.Dtos;
using Server.Models;

namespace Server.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<List<UsernameAndIdDto>> Search(string username);
        Task<User> Get(int userId);
        Task UpdateUser(User user);
        Task<List<string>> GetConnectionIdsByUserIdAndReceiverId(int receiverId, int userId);
        Task RemoveConnectionId(int userId, string connectionId);
        Task<List<UsernameAndIdDto>> GetChats(int userId);
        Task<User?> GetByUsername(string username);
    }
}
