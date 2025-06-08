using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CSE443_Project.Hubs
{
    public class JobHub : Hub
    {
        // Lưu trữ mapping giữa ConnectionId và EmployerId
        private static readonly ConcurrentDictionary<string, string> _employerConnections = new ConcurrentDictionary<string, string>();

        public async Task NewJobPosted(int jobId, string jobTitle, string company)
        {
            await Clients.All.SendAsync("ReceiveNewJob", jobId, jobTitle, company);
        }

        public async Task JobApplicationStatusChanged(string userId, int applicationId, string status)
        {
            await Clients.User(userId).SendAsync("ApplicationStatusChanged", applicationId, status);
        }

        public async Task NewJobApplication(string employerId, int jobId, string applicantName)
        {
            Console.WriteLine($"Sending notification to employer {employerId} about new application from {applicantName} for job {jobId}");

            // Gửi thông báo qua User ID (nếu đã xác thực)
            await Clients.User(employerId).SendAsync("NewApplication", jobId, applicantName);

            // Gửi thông báo đến tất cả các kết nối của employer này
            if (_employerConnections.Values.Contains(employerId))
            {
                var connections = _employerConnections
                    .Where(kvp => kvp.Value == employerId)
                    .Select(kvp => kvp.Key);

                foreach (var connectionId in connections)
                {
                    Console.WriteLine($"Sending direct notification to employer connection: {connectionId}");
                    await Clients.Client(connectionId).SendAsync("NewApplication", jobId, applicantName);
                }
            }
            else
            {
                Console.WriteLine($"No active connections found for employer {employerId}");
            }
        }

        public async Task JoinJobCategory(string category)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"category_{category}");
        }

        public async Task LeaveJobCategory(string category)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"category_{category}");
        }

        public async Task NewJobInCategory(string category, int jobId, string jobTitle)
        {
            await Clients.Group($"category_{category}").SendAsync("NewJobInCategory", category, jobId, jobTitle);
        }

        public async Task JoinAsJobSeeker(string jobSeekerId)
        {
            if (!string.IsNullOrEmpty(jobSeekerId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"jobseeker_{jobSeekerId}");
                Console.WriteLine($"JobSeeker #{jobSeekerId} đã tham gia nhóm jobseeker_{jobSeekerId}");
            }
        }

        public async Task LeaveAsJobSeeker(string jobSeekerId)
        {
            if (!string.IsNullOrEmpty(jobSeekerId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"jobseeker_{jobSeekerId}");
                Console.WriteLine($"JobSeeker #{jobSeekerId} đã rời nhóm jobseeker_{jobSeekerId}");
            }
        }

        public async Task JoinAsEmployer(string employerId)
        {
            if (!string.IsNullOrEmpty(employerId))
            {
                // Lưu mapping giữa ConnectionId và EmployerId
                _employerConnections.TryAdd(Context.ConnectionId, employerId);

                await Groups.AddToGroupAsync(Context.ConnectionId, $"employer_{employerId}");
                Console.WriteLine($"Employer #{employerId} đã tham gia nhóm employer_{employerId} với connection {Context.ConnectionId}");
            }
        }

        // Phương thức chung để tham gia vào một nhóm bất kỳ
        public async Task JoinGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                Console.WriteLine($"Connection {Context.ConnectionId} đã tham gia nhóm {groupName}");
            }
        }

        // Phương thức chung để rời khỏi một nhóm bất kỳ
        public async Task LeaveGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                Console.WriteLine($"Connection {Context.ConnectionId} đã rời nhóm {groupName}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Kết nối mới: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Ngắt kết nối: {Context.ConnectionId}");

            // Xóa mapping khi ngắt kết nối
            _employerConnections.TryRemove(Context.ConnectionId, out _);

            await base.OnDisconnectedAsync(exception);
        }
    }
}