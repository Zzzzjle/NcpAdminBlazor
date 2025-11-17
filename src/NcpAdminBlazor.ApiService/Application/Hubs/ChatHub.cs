namespace NcpAdminBlazor.ApiService.Application.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessageAsync(string user, string message);
    }

    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub<IChatClient>
    {
        public async Task SendMessageAsync(string user, string message)
        {
            await Clients.All.ReceiveMessageAsync(user, message);
        }
    }
}
