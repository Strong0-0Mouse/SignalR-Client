using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ServerSignalR
{
    public class ServerHub : Hub<IServerHub>
    {
        public static int TotalUsersConnected { get; private set; }

        public override async Task OnConnectedAsync()
        {
            TotalUsersConnected++;
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            TotalUsersConnected--;
            await base.OnDisconnectedAsync(exception);
        }
    }
}