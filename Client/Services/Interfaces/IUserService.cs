using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}