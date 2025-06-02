using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public class JobRepository : Repository<Job>, IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> GetLatestJobsAsync(int count)
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline > DateTime.Now)
                .OrderByDescending(j => j.CreatedAt)
                .Take(count)
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
                .Where(j => j.CategoryId == categoryId && j.IsActive && j.Deadline > DateTime.Now)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(string searchTerm, int? categoryId = null, string location = null)
        {
            var query = _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline > DateTime.Now);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(j =>
                    j.JobTitle.ToLower().Contains(searchTerm) ||
                    j.JobDescription.ToLower().Contains(searchTerm) ||
                    j.Requirements.ToLower().Contains(searchTerm) ||
                    j.Employer.CompanyName.ToLower().Contains(searchTerm)
                );
            }

            if (categoryId.HasValue)
            {
                query = query.Where(j => j.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(location))
            {
                location = location.ToLower();
                query = query.Where(j => j.Location.ToLower().Contains(location));
            }

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetFeaturedJobsAsync(int count)
        {
            // You might implement a featured flag on jobs or use other criteria
            // For now, we'll just return the most recent jobs with the highest salaries
            return await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Deadline > DateTime.Now)
                .OrderByDescending(j => j.SalaryMax)
                .ThenByDescending(j => j.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Job> GetJobWithDetailsAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Employer)
                    .ThenInclude(e => e.User)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}