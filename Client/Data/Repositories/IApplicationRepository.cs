using CSE443_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId);
        Task<IEnumerable<Application>> GetApplicationsByJobSeekerIdAsync(int jobSeekerId);
        Task<Application> GetApplicationWithDetailsAsync(int applicationId);
        Task<bool> HasAppliedAsync(int jobId, int jobSeekerId);
        Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status);
    }
}