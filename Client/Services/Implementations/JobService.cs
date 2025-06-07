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
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Job> GetJobByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline >= DateTime.Now)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByEmployerIdAsync(int employerId)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Where(j => j.EmployerId == employerId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByCategoryIdAsync(int categoryId)
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .Where(j => j.CategoryId == categoryId && j.IsActive && j.Deadline >= DateTime.Now)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveJobsAsync();

            searchTerm = searchTerm.ToLower();

            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline >= DateTime.Now &&
                           (j.JobTitle.ToLower().Contains(searchTerm) ||
                            j.JobDescription.ToLower().Contains(searchTerm) ||
                            j.Requirements.ToLower().Contains(searchTerm) ||
                            j.Location.ToLower().Contains(searchTerm) ||
                            j.Industry.ToLower().Contains(searchTerm) ||
                            j.Employer.CompanyName.ToLower().Contains(searchTerm) ||
                            j.Category.Name.ToLower().Contains(searchTerm)))
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> FilterJobsAsync(string location, string jobType, decimal? minSalary, decimal? maxSalary, bool? isRemote, int? categoryId)
        {
            var query = _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline >= DateTime.Now);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(j => j.Location.ToLower().Contains(location.ToLower()));

            if (!string.IsNullOrWhiteSpace(jobType))
                query = query.Where(j => j.JobType == jobType);

            if (minSalary.HasValue)
                query = query.Where(j => j.SalaryMin >= minSalary.Value);

            if (maxSalary.HasValue)
                query = query.Where(j => j.SalaryMax <= maxSalary.Value);

            if (isRemote.HasValue)
                query = query.Where(j => j.IsRemote == isRemote.Value);

            if (categoryId.HasValue)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            return await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            try
            {
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in JobService.CreateJobAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<Job> UpdateJobAsync(Job job)
        {
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return false;

            job.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalJobCountAsync()
        {
            return await _context.Jobs.CountAsync();
        }

        public async Task<int> GetActiveJobCountAsync()
        {
            return await _context.Jobs
                .CountAsync(j => j.IsActive && j.Deadline >= DateTime.Now);
        }
    }
}