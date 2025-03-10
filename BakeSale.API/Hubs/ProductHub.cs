﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API.Hubs
{
    public class ProductHub : Hub
    {
        public async Task NotifyProductUpdated(int productId)
        {
            await Clients.All.SendAsync("ProductUpdated", productId);
        }
    }
}
