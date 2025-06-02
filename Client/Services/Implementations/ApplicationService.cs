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
    public class ApplicationService : IApplicationService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.CV)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.CV)
                .ToListAsync();
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
                .Include(a => a.CV)
                .Where(a => a.JobSeekerId == jobSeekerId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.CV)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetApplicationsByEmployerIdAsync(int employerId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                .Include(a => a.CV)
                .Where(a => a.Job.EmployerId == employerId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            application.ApplicationDate = DateTime.Now;
            application.Status = "Pending";

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> UpdateApplicationAsync(Application application)
        {
            _context.Entry(application).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return false;

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateApplicationStatusAsync(int id, string status, string? notes = null)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return false;

            application.Status = status;

            if (notes != null)
                application.Notes = notes;

            if (status != "Pending")
                application.ReviewDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetApplicationCountByJobIdAsync(int jobId)
        {
            return await _context.Applications.CountAsync(a => a.JobId == jobId);
        }

        public async Task<int> GetApplicationCountByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.Applications.CountAsync(a => a.JobSeekerId == jobSeekerId);
        }

        public async Task<bool> HasJobSeekerAppliedToJobAsync(int jobSeekerId, int jobId)
        {
            return await _context.Applications.AnyAsync(a =>
                a.JobSeekerId == jobSeekerId && a.JobId == jobId);
        }
    }
}