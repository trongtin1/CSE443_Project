using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToAllAsync(string user, string message);
        Task SendPrivateNotificationAsync(string userId, string message);
        Task SendGroupNotificationAsync(string groupName, string message);
        Task NotifyNewJobPostedAsync(int jobId, string jobTitle, string company);
        Task NotifyJobApplicationStatusChangedAsync(string userId, int applicationId, string status);
        Task NotifyNewJobApplicationAsync(string employerId, int jobId, string applicantName);
        Task NotifyNewJobInCategoryAsync(string category, int jobId, string jobTitle);
    }
}