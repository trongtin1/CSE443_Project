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
    public class EmployerService : IEmployerService
    {
        private readonly ApplicationDbContext _context;

        public EmployerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employer> GetEmployerByIdAsync(int id)
        {
            return await _context.Employers
                .Include(e => e.User)
                .Include(e => e.Jobs)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employer> GetEmployerByUserIdAsync(int userId)
        {
            return await _context.Employers
                .Include(e => e.User)
                .Include(e => e.Jobs)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
        {
            return await _context.Employers
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employer> CreateEmployerAsync(Employer employer)
        {
            _context.Employers.Add(employer);
            await _context.SaveChangesAsync();
            return employer;
        }
        public async Task<Employer> UpdateEmployerAsync(Employer employer)
        {
            var existingEmployer = await _context.Employers.FindAsync(employer.Id);
            if (existingEmployer == null)
            {
                throw new InvalidOperationException($"Employer with ID {employer.Id} not found.");
            }

            // Update only the properties we want to change, not navigation properties
            existingEmployer.CompanyName = employer.CompanyName;
            existingEmployer.CompanyDescription = employer.CompanyDescription;
            existingEmployer.Industry = employer.Industry;
            existingEmployer.Website = employer.Website;
            existingEmployer.Logo = employer.Logo;
            existingEmployer.FoundedYear = employer.FoundedYear;
            existingEmployer.CompanySize = employer.CompanySize;
            // Note: UserId should not be updated after creation

            await _context.SaveChangesAsync();
            return existingEmployer;
        }

        public async Task<bool> DeleteEmployerAsync(int id)
        {
            var employer = await _context.Employers.FindAsync(id);
            if (employer == null)
                return false;

            _context.Employers.Remove(employer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetJobCountByEmployerIdAsync(int employerId)
        {
            return await _context.Jobs.CountAsync(j => j.EmployerId == employerId);
        }

        public async Task<int> GetActiveJobCountByEmployerIdAsync(int employerId)
        {
            return await _context.Jobs.CountAsync(j =>
                j.EmployerId == employerId &&
                j.IsActive &&
                j.Deadline >= DateTime.Now);
        }
    }
}