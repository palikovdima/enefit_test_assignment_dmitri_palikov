using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Configurations.Session;
using API.Controllers;
using API.Hubs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories.Product;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mocks;

namespace Tests.UnitTests.Controllers.Cart
{
    public abstract class BaseCartTest
    {
        protected readonly Mock<IHubContext<ProductHub>> _mockProductHubContext;
        protected readonly Mock<IHubContext<CartHub>> _mockCartHubContext;
        protected readonly Mock<ILogger<CartController>> _mockLogger;
        protected readonly Mock<ISessionWrapper> _mockSessionWrapper;
        protected readonly Mock<ProductRepository> _productRepositoryMock;

        protected readonly CartController _controller;
        protected const string SessionKeyCart = "Cart";

        protected BaseCartTest()
        {
            _mockProductHubContext = new Mock<IHubContext<ProductHub>>();
            _mockCartHubContext = new Mock<IHubContext<CartHub>>();
            _mockLogger = new Mock<ILogger<CartController>>();
            _mockSessionWrapper = new Mock<ISessionWrapper>();
            _productRepositoryMock = Mocks.MockRepository.GetProductRepository(GetType());
            _controller = new CartController(
                _productRepositoryMock.Object,
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
