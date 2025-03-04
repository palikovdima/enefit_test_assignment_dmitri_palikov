using BakeSale.API.Controllers;
using BakeSale.API.Helpers;
using BakeSale.Domain.Entities;
using BakeSale.Domain.Interfaces;
using BakeSale.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BakeSale.Tests.UnitTests.Cart.Add
{
    public class AddToCartTests : BaseCartTest
    {
        [Fact]
        public async Task AddToCart_WhenProductExists_ShouldIncreaseCartCount()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 10 };

            _mockProductRepository.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<OkObjectResult>();
            _mockProductRepository.Verify(r => r.FindAsync(product.Id), Times.Once);
            VerifyClientProxies();
        }

        [Fact]
        public async Task AddToCart_ProductDoesNotExist_ReturnsBadRequest()
        {
            var productId = 1;
            _mockProductRepository.Setup(repo => repo.FindAsync(productId)).ReturnsAsync((Product?)null);

            var result = await _controller.AddToCart(productId);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Invalid product data.");
            
        }

        [Fact]
        public async Task AddToCart_WhenProductQuantityIsZero_ShouldReturnBadRequest()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 0 };

            _mockProductRepository.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Product is out of stock.");
        }

        [Fact]
        public async Task AddToCart_WhenProductDataIsUnavailable_ShouldReturnBadRequest()
        {
            var productId = 1;

            _mockProductRepository.Setup(r => r.FindAsync(productId)).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.AddToCart(productId);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Invalid product data.");
        }

        [Fact]
        public async Task AddToCart_WhenCartIsNull_ShouldAddProductSuccessfully()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 10 };

            _mockProductRepository.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns((List<Product>?)null!);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<OkObjectResult>();
            _mockSessionWrapper.Verify(s => s.SetObject(SessionKeyCart, It.IsAny<List<Product>>()), Times.Once);
        }
    }
}
