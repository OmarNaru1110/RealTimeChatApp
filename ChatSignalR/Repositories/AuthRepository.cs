using Microsoft.EntityFrameworkCore;
using Server.Dtos;
using Server.Models;
using Server.Repositories.Context;
using Server.Repositories.IRepositories;

namespace Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationContext _context;

        public AuthRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<bool> Register(UserDto user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return false;
            var userExist = _context.Users.Any(u => u.Username == user.Username);
            if (userExist == true)
                return false;

            var newUser = new User { Username =  user.Username, Password = user.Password };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> SignIn(UserDto user)
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
            return result == null? null : new UserDto { Id = result.Id, Username = result.Username, Password = result.Password };
        }

    }
}
