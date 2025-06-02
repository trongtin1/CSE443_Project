using CSE443_Project.Data.Repositories;
using CSE443_Project.Models;
using System;
using System.Threading.Tasks;

namespace CSE443_Project.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        private IUserRepository _userRepository;
        private IJobRepository _jobRepository;
        private IApplicationRepository _applicationRepository;
        private IRepository<JobCategory> _jobCategoryRepository;
        private IRepository<CV> _cvRepository;
        private IRepository<SaveJob> _saveJobRepository;
        private IRepository<Candidate> _candidateRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_context);

        public IJobRepository JobRepository =>
            _jobRepository ??= new JobRepository(_context);

        public IApplicationRepository ApplicationRepository =>
            _applicationRepository ??= new ApplicationRepository(_context);

        public IRepository<JobCategory> JobCategoryRepository =>
            _jobCategoryRepository ??= new Repository<JobCategory>(_context);

        public IRepository<CV> CVRepository =>
            _cvRepository ??= new Repository<CV>(_context);

        public IRepository<SaveJob> SaveJobRepository =>
            _saveJobRepository ??= new Repository<SaveJob>(_context);

        public IRepository<Candidate> CandidateRepository =>
            _candidateRepository ??= new Repository<Candidate>(_context);


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}