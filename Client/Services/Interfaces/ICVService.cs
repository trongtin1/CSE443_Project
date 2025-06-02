using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface ICVService
    {
        Task<CV> GetCVByIdAsync(int id);
        Task<IEnumerable<CV>> GetAllCVsAsync();
        Task<IEnumerable<CV>> GetCVsByJobSeekerIdAsync(int jobSeekerId);
        Task<CV> GetDefaultCVByJobSeekerIdAsync(int jobSeekerId);
        Task<CV> CreateCVAsync(CV cv);
        Task<CV> UpdateCVAsync(CV cv);
        Task<bool> DeleteCVAsync(int id);
        Task<bool> SetDefaultCVAsync(int cvId, int jobSeekerId);
    }
}