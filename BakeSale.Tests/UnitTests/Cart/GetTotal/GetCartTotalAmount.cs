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
using Microsoft.Extensions.Logging;
using Moq;

namespace BakeSale.Tests.UnitTests.Cart.GetTotal
{
    public class GetCartTotalAmountTests : BaseCartTest
    {
        [Fact]
        public void GetCartTotalAmount_CartExists_ReturnsTotalAmount()
        {
            decimal price = 10;
            int quantity = 2;
            decimal totalAmount = price * quantity;

            var cart = new List<Product> { new Product { Id = 1, Price = price, Quantity = quantity } };
            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns(cart);

            var result = _controller.GetCartTotalAmount();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(totalAmount);
        }

        [Fact]
        public void GetCartTotalAmount_WhenMultipleProductsExist_ShouldReturnCorrectTotalAmount()
        {
            var cart = new List<Product>
            {
                new Product { Id = 1, Price = 10m, Quantity = 2 },
                new Product { Id = 2, Price = 5m, Quantity = 3 }
            };

            decimal expectedTotal = 10m * 2 + 5m * 3; // 20 + 15 = 35

            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns(cart);

            var result = _controller.GetCartTotalAmount();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedTotal);
        }

        [Fact]
        public void GetCartTotalAmount_CartDoesNotExist_ReturnsZero()
        {
            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns((List<Product>?)null!);

            var result = _controller.GetCartTotalAmount();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(0);
        }
    }
}
