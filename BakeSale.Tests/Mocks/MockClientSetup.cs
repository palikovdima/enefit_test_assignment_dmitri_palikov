using BakeSale.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace BakeSale.Tests.Mocks;
public class MockClientSetup
{
    public static void SetupMockClients(Mock<IHubContext<ProductHub>> mockProductHubContext, Mock<IHubContext<CartHub>> mockCartHubContext)
    {
        var mockProductClientProxy = new Mock<IClientProxy>();
        var mockProductClients = new Mock<IHubClients>();
        mockProductClients.Setup(clients => clients.All).Returns(mockProductClientProxy.Object);

        var mockCartClientProxy = new Mock<IClientProxy>();
        var mockCartClients = new Mock<IHubClients>();
        mockCartClients.Setup(clients => clients.All).Returns(mockCartClientProxy.Object);

        mockProductHubContext.Setup(hub => hub.Clients).Returns(mockProductClients.Object);
        mockCartHubContext.Setup(hub => hub.Clients).Returns(mockCartClients.Object);
    }
}

