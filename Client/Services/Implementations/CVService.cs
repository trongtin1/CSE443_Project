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
    public class CVService : ICVService
    {
        private readonly ApplicationDbContext _context;

        public CVService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CV> GetCVByIdAsync(int id)
        {
            return await _context.CVs
                .Include(cv => cv.JobSeeker)
                    .ThenInclude(js => js.User)
                .FirstOrDefaultAsync(cv => cv.Id == id);
        }

        public async Task<IEnumerable<CV>> GetAllCVsAsync()
        {
            return await _context.CVs
                .Include(cv => cv.JobSeeker)
                    .ThenInclude(js => js.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<CV>> GetCVsByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.CVs
                .Where(cv => cv.JobSeekerId == jobSeekerId)
                .Include(cv => cv.JobSeeker)
                .OrderByDescending(cv => cv.CreatedAt)
                .ToListAsync();
        }

        public async Task<CV> GetDefaultCVByJobSeekerIdAsync(int jobSeekerId)
        {
            return await _context.CVs
                .FirstOrDefaultAsync(cv => cv.JobSeekerId == jobSeekerId && cv.IsDefault);
        }

        public async Task<CV> CreateCVAsync(CV cv)
        {
            cv.CreatedAt = DateTime.Now;

            // If this is the first CV or it's marked as default
            if (cv.IsDefault || !await _context.CVs.AnyAsync(c => c.JobSeekerId == cv.JobSeekerId))
            {
                // Ensure only one CV is set as default
                await SetAllCVsAsNonDefaultAsync(cv.JobSeekerId);
                cv.IsDefault = true;
            }

            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();
            return cv;
        }
        public async Task<CV> UpdateCVAsync(CV cv)
        {
            cv.UpdatedAt = DateTime.Now;

            if (cv.IsDefault)
            {
                // Ensure only one CV is set as default
                await SetAllCVsAsNonDefaultAsync(cv.JobSeekerId);
                // Ensure the current CV is set as default
                cv.IsDefault = true;
            }

            _context.Entry(cv).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return cv;
        }

        public async Task<bool> DeleteCVAsync(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
                return false;

            bool wasDefault = cv.IsDefault;
            int jobSeekerId = cv.JobSeekerId;

            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();

            // If the deleted CV was the default, set another one as default if any exist
            if (wasDefault)
            {
                var anotherCV = await _context.CVs
                    .FirstOrDefaultAsync(c => c.JobSeekerId == jobSeekerId);

                if (anotherCV != null)
                {
                    anotherCV.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> SetDefaultCVAsync(int cvId, int jobSeekerId)
        {
            // Ensure the CV belongs to the jobSeeker
            var cv = await _context.CVs
                .FirstOrDefaultAsync(c => c.Id == cvId && c.JobSeekerId == jobSeekerId);

            if (cv == null)
                return false;

            // Set all CVs of this jobSeeker to non-default
            await SetAllCVsAsNonDefaultAsync(jobSeekerId);

            // Set the selected CV as default
            cv.IsDefault = true;
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task SetAllCVsAsNonDefaultAsync(int jobSeekerId)
        {
            var cvs = await _context.CVs
                .Where(c => c.JobSeekerId == jobSeekerId)
                .ToListAsync();

            foreach (var cv in cvs)
            {
                cv.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}