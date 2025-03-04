using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BakeSale.API.Hubs
{
    public class CartHub : Hub
    {
        public async Task NotifyCartUpdated()
        {
            await Clients.All.SendAsync("CartUpdated");
        }
    }
}
