using CSE443_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> AuthenticateAsync(string username, string password);
        Task<JobSeeker> GetJobSeekerByUserIdAsync(int userId);
        Task<Employer> GetEmployerByUserIdAsync(int userId);
    }
}