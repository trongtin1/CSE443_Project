using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IEmployerService
    {
        Task<Employer> GetEmployerByIdAsync(int id);
        Task<Employer> GetEmployerByUserIdAsync(int userId);
        Task<IEnumerable<Employer>> GetAllEmployersAsync();
        Task<Employer> CreateEmployerAsync(Employer employer);
        Task<Employer> UpdateEmployerAsync(Employer employer);
        Task<bool> DeleteEmployerAsync(int id);
        Task<int> GetJobCountByEmployerIdAsync(int employerId);
        Task<int> GetActiveJobCountByEmployerIdAsync(int employerId);
        Task<int> GetAllEmployersCountAsync();
    }
}