using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface ICandidateService
    {
        Task<Candidate> GetCandidateByIdAsync(int id);
        Task<IEnumerable<Candidate>> GetAllCandidatesAsync();
        Task<IEnumerable<Candidate>> GetCandidatesByJobIdAsync(int jobId);
        Task<IEnumerable<Candidate>> GetCandidatesByJobSeekerIdAsync(int jobSeekerId);
        Task<IEnumerable<Candidate>> GetCandidatesByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Candidate>> GetCandidatesByStatusAsync(string status);
        Task<Candidate> CreateCandidateAsync(Candidate candidate);
        Task<Candidate> UpdateCandidateAsync(Candidate candidate);
        Task<bool> DeleteCandidateAsync(int id);
        Task<bool> UpdateCandidateStatusAsync(int id, string status, string? interviewNotes = null, DateTime? interviewDate = null);
        Task<int> GetCandidateCountByJobIdAsync(int jobId);
        Task<int> GetCandidateCountByStatusAsync(string status);
    }
}