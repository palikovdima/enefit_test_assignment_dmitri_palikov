using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeSale.API.Controllers;
using BakeSale.API.Helpers;
using BakeSale.API.Hubs;
using BakeSale.Domain.Entities;
using BakeSale.Domain.Interfaces;
using BakeSale.Tests.Mocks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace BakeSale.Tests.UnitTests.Cart
{
    public abstract class BaseCartTest
    {
        protected readonly Mock<IRepository<Product>> _mockProductRepository;
        protected readonly Mock<IHubContext<ProductHub>> _mockProductHubContext;
        protected readonly Mock<IHubContext<CartHub>> _mockCartHubContext;
        protected readonly Mock<ILogger<CartController>> _mockLogger;
        protected readonly Mock<ISessionWrapper> _mockSessionWrapper;
        protected readonly CartController _controller;
        protected const string SessionKeyCart = "Cart";

        protected BaseCartTest()
        {
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockProductHubContext = new Mock<IHubContext<ProductHub>>();
            _mockCartHubContext = new Mock<IHubContext<CartHub>>();
            _mockLogger = new Mock<ILogger<CartController>>();
            _mockSessionWrapper = new Mock<ISessionWrapper>();

            _controller = new CartController(
                _mockProductRepository.Object,
                _mockProductHubContext.Object,
                _mockCartHubContext.Object,
                _mockLogger.Object,
                _mockSessionWrapper.Object
            );

            MockClientSetup.SetupMockClients(_mockProductHubContext, _mockCartHubContext);
        }

        protected void VerifyClientProxies()
        {
            ClientVerificationHelper.VerifyClientProxies(_mockProductHubContext, _mockCartHubContext);
        }
    }
}
