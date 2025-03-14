using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Tests.UnitTests.Controllers.Cart;

namespace Tests.UnitTests.Controllers.Cart.Get
{
    public class GetCartTests : BaseCartTest
    {
        [Fact]
        public void GetCart_CartExists_ReturnsCart()
        {
            var cart = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product", Price = 10, Quantity = 1 }
            };

            var cartJson = JsonConvert.SerializeObject(cart);

            _mockSessionWrapper.Setup(s => s.GetString(SessionKeyCart)).Returns(cartJson);

            var result = _controller.GetCart();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(cart);
        }

        [Fact]
        public void GetCart_CartDoesNotExist_ReturnsEmptyCart()
        {
            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns((List<Product>?)null!);

            var result = _controller.GetCart();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(new List<Product>());
        }
    }
}
