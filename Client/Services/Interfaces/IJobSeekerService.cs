using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IJobSeekerService
    {
        Task<JobSeeker> GetJobSeekerByIdAsync(int id);
        Task<JobSeeker> GetJobSeekerByUserIdAsync(int userId);
        Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync();
        Task<JobSeeker> CreateJobSeekerAsync(JobSeeker jobSeeker);
        Task<JobSeeker> UpdateJobSeekerAsync(JobSeeker jobSeeker);
        Task<bool> DeleteJobSeekerAsync(int id);
        Task<int> GetApplicationCountByJobSeekerIdAsync(int jobSeekerId);
        Task<int> GetSavedJobCountByJobSeekerIdAsync(int jobSeekerId);
    }
}