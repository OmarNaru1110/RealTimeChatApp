using Server.Dtos;
using Server.Models;

namespace Server.Repositories.IRepositories
{
    public interface IAuthRepository
    {
        Task<bool> Register(UserDto user);
        Task<UserDto?> SignIn(UserDto user);
    }
}
