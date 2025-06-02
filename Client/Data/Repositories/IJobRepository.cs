using CSE443_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<IEnumerable<Job>> GetLatestJobsAsync(int count);
        Task<IEnumerable<Job>> GetJobsByEmployerIdAsync(int employerId);
        Task<IEnumerable<Job>> GetJobsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Job>> SearchJobsAsync(string searchTerm, int? categoryId = null, string location = null);
        Task<IEnumerable<Job>> GetFeaturedJobsAsync(int count);
        Task<Job> GetJobWithDetailsAsync(int id);
    }
}