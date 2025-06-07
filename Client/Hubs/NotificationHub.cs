using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CSE443_Project.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public async Task SendPrivateNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceivePrivateNotification", message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendGroupNotification(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}