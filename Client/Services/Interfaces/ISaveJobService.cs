using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface ISaveJobService
    {
        Task<SaveJob> GetSaveJobByIdAsync(int id);
        Task<IEnumerable<SaveJob>> GetAllSaveJobsAsync();
        Task<IEnumerable<SaveJob>> GetSaveJobsByJobSeekerIdAsync(int jobSeekerId);
        Task<IEnumerable<SaveJob>> GetSaveJobsByJobIdAsync(int jobId);
        Task<SaveJob> CreateSaveJobAsync(SaveJob saveJob);
        Task<SaveJob> UpdateSaveJobAsync(SaveJob saveJob);
        Task<bool> DeleteSaveJobAsync(int id);
        Task<bool> HasJobSeekerSavedJobAsync(int jobSeekerId, int jobId);
        Task<SaveJob> GetSaveJobByJobSeekerAndJobIdAsync(int jobSeekerId, int jobId);
    }
}