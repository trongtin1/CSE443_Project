using CSE443_Project.Hubs;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
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
            try
            {
                // Gửi thông báo qua User ID (yêu cầu xác thực)
                await _jobHubContext.Clients.User(userId).SendAsync("ApplicationStatusChanged", applicationId, status);

                // Gửi thông báo qua nhóm JobSeeker (không yêu cầu xác thực)
                await _jobHubContext.Clients.Group($"jobseeker_{userId}").SendAsync("ApplicationStatusChanged", applicationId, status);

                // Ghi log
                Console.WriteLine($"Đã gửi thông báo thay đổi trạng thái đơn ứng tuyển #{applicationId} thành '{status}' đến JobSeeker #{userId}");

                // Gửi thêm thông báo chung để đảm bảo
                await _notificationHubContext.Clients.All.SendAsync("JobSeekerNotification", userId, $"Đơn ứng tuyển #{applicationId} của bạn đã được cập nhật thành {status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi thông báo đến JobSeeker: {ex.Message}");
            }
        }

        public async Task NotifyNewJobApplicationAsync(string employerId, int jobId, string applicantName)
        {
            try
            {
                Console.WriteLine($"Sending notification to employer {employerId} about new application from {applicantName} for job {jobId}");

                // Gửi thông báo qua JobHub (kênh chính)
                await _jobHubContext.Clients.User(employerId).SendAsync("NewApplication", jobId, applicantName);

                // Gửi thông báo qua nhóm Employer
                await _jobHubContext.Clients.Group($"employer_{employerId}").SendAsync("NewApplication", jobId, applicantName);

                // Gửi thông báo qua NotificationHub (kênh phụ)
                await _notificationHubContext.Clients.User(employerId).SendAsync("ReceivePrivateNotification",
                    $"Ứng viên {applicantName} đã ứng tuyển vào công việc #{jobId} của bạn");

                Console.WriteLine($"Notification sent successfully to employer {employerId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification to employer: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task NotifyNewJobInCategoryAsync(string category, int jobId, string jobTitle)
        {
            await _jobHubContext.Clients.Group($"category_{category}").SendAsync("NewJobInCategory", category, jobId, jobTitle);
        }
    }
}