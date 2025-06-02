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
    public class SaveJobService : ISaveJobService
    {
        private readonly ApplicationDbContext _context;

        public SaveJobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SaveJob> GetSaveJobByIdAsync(int id)
        {
            return await _context.SavedJobs
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Employer)
                .Include(sj => sj.JobSeeker)
                    .ThenInclude(js => js.User)
                .FirstOrDefaultAsync(sj => sj.Id == id);
        }

        public async Task<IEnumerable<SaveJob>> GetAllSaveJobsAsync()
        {
            return await _context.SavedJobs
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Employer)
                .Include(sj => sj.JobSeeker)
                    .ThenInclude(js => js.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaveJob>> GetSaveJobsByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.SavedJobs
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Employer)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Category)
                .Where(sj => sj.JobSeekerId == jobSeekerId)
                .OrderByDescending(sj => sj.SavedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaveJob>> GetSaveJobsByJobIdAsync(int jobId)
        {
            return await _context.SavedJobs
                .Include(sj => sj.JobSeeker)
                    .ThenInclude(js => js.User)
                .Where(sj => sj.JobId == jobId)
                .OrderByDescending(sj => sj.SavedAt)
                .ToListAsync();
        }

        public async Task<SaveJob> CreateSaveJobAsync(SaveJob saveJob)
        {
            saveJob.SavedAt = DateTime.Now;

            _context.SavedJobs.Add(saveJob);
            await _context.SaveChangesAsync();
            return saveJob;
        }

        public async Task<SaveJob> UpdateSaveJobAsync(SaveJob saveJob)
        {
            _context.Entry(saveJob).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return saveJob;
        }

        public async Task<bool> DeleteSaveJobAsync(int id)
        {
            var saveJob = await _context.SavedJobs.FindAsync(id);
            if (saveJob == null)
                return false;

            _context.SavedJobs.Remove(saveJob);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasJobSeekerSavedJobAsync(int jobSeekerId, int jobId)
        {
            return await _context.SavedJobs.AnyAsync(sj =>
                sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);
        }

        public async Task<SaveJob> GetSaveJobByJobSeekerAndJobIdAsync(int jobSeekerId, int jobId)
        {
            return await _context.SavedJobs
                .FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);
        }
    }
}