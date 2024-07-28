using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace PRN221_SocialMedia.Hub
{
    public class MessageHub : Microsoft.AspNetCore.SignalR.Hub
    {
        // A thread-safe dictionary to map user IDs to connection IDs
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        // Method to set the user ID for the current connection
        public Task SetUserId(string userId)
        {
            UserConnections[Context.ConnectionId] = userId;
            return Task.CompletedTask;
        }

        // When a client disconnects, remove their connection ID from the dictionary
        // When a client disconnects, remove their connection ID from the dictionary
        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserConnections.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }

        // Send a message to a specific user
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            // Find the connection ID for the receiver
            var receiverConnectionId = UserConnections.FirstOrDefault(x => x.Value == receiverId).Key;

            if (receiverConnectionId != null)
            {
                // Send message to receiver
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message);
            }
        }

    }
}
