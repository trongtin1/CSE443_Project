using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // In a real application, you would hash the password and compare hashes
            // This is a simplified version for demonstration purposes
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);

            return user;
        }

        public async Task<JobSeeker> GetJobSeekerByUserIdAsync(int userId)
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .Include(js => js.CVs)
                .FirstOrDefaultAsync(js => js.UserId == userId);
        }

        public async Task<Employer> GetEmployerByUserIdAsync(int userId)
        {
            return await _context.Employers
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}