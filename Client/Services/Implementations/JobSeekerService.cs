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
    public class JobSeekerService : IJobSeekerService
    {
        private readonly ApplicationDbContext _context;

        public JobSeekerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<JobSeeker> GetJobSeekerByIdAsync(int id)
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .Include(js => js.CVs)
                .Include(js => js.Applications)
                .Include(js => js.SavedJobs)
                .FirstOrDefaultAsync(js => js.Id == id);
        }
        public async Task<JobSeeker> GetJobSeekerByUserIdAsync(int userId)
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .Include(js => js.CVs)
                .Include(js => js.Applications)
                .Include(js => js.SavedJobs)
                .FirstOrDefaultAsync(js => js.UserId == userId);
        }

        public async Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync()
        {
            return await _context.JobSeekers
                .Include(js => js.User)
                .ToListAsync();
        }

        public async Task<JobSeeker> CreateJobSeekerAsync(JobSeeker jobSeeker)
        {
            _context.JobSeekers.Add(jobSeeker);
            await _context.SaveChangesAsync();
            return jobSeeker;
        }

        public async Task<JobSeeker> UpdateJobSeekerAsync(JobSeeker jobSeeker)
        {
            _context.Entry(jobSeeker).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return jobSeeker;
        }

        public async Task<bool> DeleteJobSeekerAsync(int id)
        {
            var jobSeeker = await _context.JobSeekers.FindAsync(id);
            if (jobSeeker == null)
                return false;

            _context.JobSeekers.Remove(jobSeeker);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetApplicationCountByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.Applications.CountAsync(a => a.JobSeekerId == jobSeekerId);
        }

        public async Task<int> GetSavedJobCountByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.SavedJobs.CountAsync(sj => sj.JobSeekerId == jobSeekerId);
        }
    }
}