using CSE443_Project.Data;
using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            // In a real application, you would hash the password and compare with stored hash
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == username &&
                u.Password == password &&
                u.IsActive);

            return user != null;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}