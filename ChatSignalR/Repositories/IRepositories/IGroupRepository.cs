using Server.Dtos;
using Server.Models;

namespace Server.Repositories.IRepositories
{
    public interface IGroupRepository
    {
        Task<GroupDto> GetGroup(int groupId);
        Task<Group> Get(int groupId);
        Task<List<GroupDto>> GetUserGroups(int userId);
        Task<List<MessageDto>> GetGroupMessages(int groupId);
        Task AddGroup(GroupDto dto);
        Task UpdateGroup(GroupDto dto);
        Task UpdateGroup(Group group);
        Task RemoveConnectionId(int groupId, string connectionId);
        Task<List<string>?> GetGroupConnections(int groupId);   
    }
}
