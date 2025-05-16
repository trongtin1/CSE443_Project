namespace Admin.Models.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalCandidates { get; set; }
        public int ActiveCandidates { get; set; }
        public int NewCandidatesToday { get; set; }
        
        public int TotalEmployers { get; set; }
        public int VerifiedEmployers { get; set; }
        public int NewEmployersToday { get; set; }
        
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int ExpiredJobs { get; set; }
        public int NewJobsToday { get; set; }
        
        public int TotalApplications { get; set; }
        public int NewApplicationsToday { get; set; }
        
        public List<ChartData> CandidateRegistrationsChart { get; set; }
        public List<ChartData> JobPostingsChart { get; set; }
        public List<ChartData> ApplicationsChart { get; set; }
    }
    
    public class ChartData
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
} 