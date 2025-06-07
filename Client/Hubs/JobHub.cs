using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CSE443_Project.Hubs
{
    public class JobHub : Hub
    {
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
            await Clients.User(employerId).SendAsync("NewApplication", jobId, applicantName);
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
    }
}