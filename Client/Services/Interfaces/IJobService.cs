using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IJobService
    {
        Task<Job> GetJobByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetJobsByEmployerIdAsync(int employerId);
        Task<IEnumerable<Job>> GetJobsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Job>> SearchJobsAsync(string searchTerm);
        Task<IEnumerable<Job>> FilterJobsAsync(string location, string jobType, decimal? minSalary, decimal? maxSalary, bool? isRemote, int? categoryId);
        Task<Job> CreateJobAsync(Job job);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(int id);
        Task<bool> DeactivateJobAsync(int id);
        Task<int> GetTotalJobCountAsync();
        Task<int> GetActiveJobCountAsync();
    }
}