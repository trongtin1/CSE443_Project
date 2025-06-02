using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.CV)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Where(a => a.JobSeekerId == jobSeekerId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Application> GetApplicationWithDetailsAsync(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.CV)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<bool> HasAppliedAsync(int jobId, int jobSeekerId)
        {
            return await _context.Applications
                .AnyAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId);
        }

        public async Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status)
        {
            return await _context.Applications
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }
    }
}