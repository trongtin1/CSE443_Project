using CSE443_Project.Data;
using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ApplicationDbContext _context;

        public CandidateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id)
        {
            return await _context.Candidates
                .Include(c => c.Application)
                .Include(c => c.Job)
                    .ThenInclude(j => j.Employer)
                .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            return await _context.Candidates
                .Include(c => c.Application)
                .Include(c => c.Job)
                    .ThenInclude(j => j.Employer)
                .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesByJobIdAsync(int jobId)
        {
            return await _context.Candidates
                .Include(c => c.Application)
                .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                .Where(c => c.JobId == jobId)
                .OrderByDescending(c => c.ShortlistedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.Candidates
                .Include(c => c.Application)
                .Include(c => c.Job)
                    .ThenInclude(j => j.Employer)
                .Where(c => c.JobSeekerId == jobSeekerId)
                .OrderByDescending(c => c.ShortlistedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesByApplicationIdAsync(int applicationId)
        {
            return await _context.Candidates
                .Include(c => c.Job)
                    .ThenInclude(j => j.Employer)
                .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                .Where(c => c.ApplicationId == applicationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesByStatusAsync(string status)
        {
            return await _context.Candidates
                .Include(c => c.Application)
                .Include(c => c.Job)
                    .ThenInclude(j => j.Employer)
                .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.ShortlistedDate)
                .ToListAsync();
        }

        public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
        {
            candidate.ShortlistedDate = DateTime.Now;

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
        {
            _context.Entry(candidate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<bool> DeleteCandidateAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return false;

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCandidateStatusAsync(int id, string status, string? interviewNotes = null, DateTime? interviewDate = null)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return false;

            candidate.Status = status;

            if (interviewNotes != null)
                candidate.InterviewNotes = interviewNotes;

            if (interviewDate.HasValue)
                candidate.InterviewDate = interviewDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCandidateCountByJobIdAsync(int jobId)
        {
            return await _context.Candidates.CountAsync(c => c.JobId == jobId);
        }

        public async Task<int> GetCandidateCountByStatusAsync(string status)
        {
            return await _context.Candidates.CountAsync(c => c.Status == status);
        }
    }
}