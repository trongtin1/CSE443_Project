using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationByIdAsync(int id);
        Task<IEnumerable<Application>> GetAllApplicationsAsync();
        Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId);
        Task<IEnumerable<Application>> GetApplicationsByJobSeekerIdAsync(int jobSeekerId);
        Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status);
        Task<IEnumerable<Application>> GetApplicationsByEmployerIdAsync(int employerId);
        Task<Application> CreateApplicationAsync(Application application);
        Task<Application> UpdateApplicationAsync(Application application);
        Task<bool> DeleteApplicationAsync(int id);
        Task<bool> UpdateApplicationStatusAsync(int id, string status, string? notes = null);
        Task<int> GetApplicationCountByJobIdAsync(int jobId);
        Task<int> GetApplicationCountByJobSeekerIdAsync(int jobSeekerId);
        Task<bool> HasJobSeekerAppliedToJobAsync(int jobSeekerId, int jobId);
    }
}