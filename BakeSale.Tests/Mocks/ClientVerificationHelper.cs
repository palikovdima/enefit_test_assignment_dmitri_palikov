using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeSale.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace BakeSale.Tests.Mocks
{
    public static class ClientVerificationHelper
    {
        public static void VerifyClientProxies(
            Mock<IHubContext<ProductHub>> mockProductHubContext,
            Mock<IHubContext<CartHub>> mockCartHubContext)
        {
            mockProductHubContext.Verify(hub => hub.Clients.All.SendCoreAsync(
                "ProductUpdated",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            mockCartHubContext.Verify(hub => hub.Clients.All.SendCoreAsync(
                "CartUpdated",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
