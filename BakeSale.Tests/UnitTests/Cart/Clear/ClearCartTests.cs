using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeSale.API.Controllers;
using BakeSale.API.Helpers;
using BakeSale.Domain.Entities;
using BakeSale.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace BakeSale.Tests.UnitTests.Cart.Clear
{
    public class ClearCartTests : BaseCartTest
    {
        [Fact]
        public async Task ClearCart_CartExists_ClearsCart()
        {
            var cart = new List<Product> { new Product { Id = 1, Price = 10, Quantity = 2 } };
            var cartJson = JsonConvert.SerializeObject(cart);

            _mockSessionWrapper.Setup(s => s.GetString(SessionKeyCart)).Returns(cartJson);

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            _mockCartHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);

            var result = await _controller.ClearCart();

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be("Cart cleared.");

            _mockSessionWrapper.Verify(s => s.Remove(SessionKeyCart), Times.Once);

            mockClientProxy.Verify(c => c.SendCoreAsync(
                "CartUpdated",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }


        [Fact]
        public async Task ClearCart_WhenCartHasManyItems_ShouldClearSuccessfully()
        {
            var cart = new List<Product>();
            for (int i = 0; i < 1000; i++)
            {
                cart.Add(new Product { Id = i + 1, Price = 1.99m, Quantity = 2 });
            }

            var cartJson = JsonConvert.SerializeObject(cart);

            _mockSessionWrapper.Setup(s => s.GetString(SessionKeyCart)).Returns(cartJson);

            var result = await _controller.ClearCart();

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be("Cart cleared.");
            _mockSessionWrapper.Verify(s => s.Remove(SessionKeyCart), Times.Once);
        }

        [Fact]
        public async Task ClearCart_CartDoesNotExist_ReturnsAlreadyEmptyMessage()
        {
            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns((List<Product>?)null!);

            var result = await _controller.ClearCart();

            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be("Cart is already empty.");
        }
    }
}
