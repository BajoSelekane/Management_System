namespace Web_app.NotificationHub
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class MessageHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Notify the specific user about the new message
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public async Task UpdateMessageCount(string user, int count)
        {
            // Update the message count for the specific user
            await Clients.User(user).SendAsync("UpdateMessageCount", count);
        }
    }
}
