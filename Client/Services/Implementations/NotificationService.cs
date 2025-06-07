using CSE443_Project.Hubs;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IHubContext<JobHub> _jobHubContext;

        public NotificationService(
            IHubContext<NotificationHub> notificationHubContext,
            IHubContext<JobHub> jobHubContext)
        {
            _notificationHubContext = notificationHubContext;
            _jobHubContext = jobHubContext;
        }

        public async Task SendNotificationToAllAsync(string user, string message)
        {
            await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public async Task SendPrivateNotificationAsync(string userId, string message)
        {
            await _notificationHubContext.Clients.User(userId).SendAsync("ReceivePrivateNotification", message);
        }

        public async Task SendGroupNotificationAsync(string groupName, string message)
        {
            await _notificationHubContext.Clients.Group(groupName).SendAsync("ReceiveGroupNotification", message);
        }

        public async Task NotifyNewJobPostedAsync(int jobId, string jobTitle, string company)
        {
            await _jobHubContext.Clients.All.SendAsync("ReceiveNewJob", jobId, jobTitle, company);
        }

        public async Task NotifyJobApplicationStatusChangedAsync(string userId, int applicationId, string status)
        {
            await _jobHubContext.Clients.User(userId).SendAsync("ApplicationStatusChanged", applicationId, status);
        }

        public async Task NotifyNewJobApplicationAsync(string employerId, int jobId, string applicantName)
        {
            await _jobHubContext.Clients.User(employerId).SendAsync("NewApplication", jobId, applicantName);
        }

        public async Task NotifyNewJobInCategoryAsync(string category, int jobId, string jobTitle)
        {
            await _jobHubContext.Clients.Group($"category_{category}").SendAsync("NewJobInCategory", category, jobId, jobTitle);
        }
    }
}