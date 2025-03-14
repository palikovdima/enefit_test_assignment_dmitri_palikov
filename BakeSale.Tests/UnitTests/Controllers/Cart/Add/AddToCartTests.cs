using API.Controllers;
using Domain.Interfaces;
using Tests.Mocks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tests.UnitTests.Controllers.Cart;
using Xunit.Abstractions;

namespace Tests.UnitTests.Controllers.Cart.Add
{
    public class AddToCartTests : BaseCartTest
    {
        private readonly ITestOutputHelper _output;

        public AddToCartTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task AddToCart_WhenProductExists_ShouldIncreaseCartCount()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 10 };

            await _productRepositoryMock.Object.AddAsync(product);

            var products = await _productRepositoryMock.Object.AllAsync();

            _output.WriteLine($"Products in DB: {string.Join(", ", products.Select(p => p.Id))}");

            _productRepositoryMock.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<OkObjectResult>();
            _productRepositoryMock.Verify(r => r.FindAsync(product.Id), Times.Once);
            VerifyClientProxies();
        }

        [Fact]
        public async Task AddToCart_ProductDoesNotExist_ReturnsBadRequest()
        {
            var productId = 1;
            _productRepositoryMock.Setup(repo => repo.FindAsync(productId)).ReturnsAsync((Product?)null);

            var result = await _controller.AddToCart(productId);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Invalid product data.");
            
        }

        [Fact]
        public async Task AddToCart_WhenProductQuantityIsZero_ShouldReturnBadRequest()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 0 };

            _productRepositoryMock.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Product is out of stock.");
        }

        [Fact]
        public async Task AddToCart_WhenProductDataIsUnavailable_ShouldReturnBadRequest()
        {
            var productId = 1;

            _productRepositoryMock.Setup(r => r.FindAsync(productId)).ThrowsAsync(new Exception("Database error"));

            var result = await _controller.AddToCart(productId);

            result.Should().BeOfType<BadRequestObjectResult>()
                    .Which.Value.Should().Be("Invalid product data.");
        }

        [Fact]
        public async Task AddToCart_WhenCartIsNull_ShouldAddProductSuccessfully()
        {
            var product = new Product { Id = 1, Name = "Brownie", Price = 0.65m, Quantity = 10 };

            await _productRepositoryMock.Object.AddAsync(product);

            _productRepositoryMock.Setup(r => r.FindAsync(product.Id)).ReturnsAsync(product);

            _mockSessionWrapper.Setup(s => s.GetObject<List<Product>>(SessionKeyCart)).Returns((List<Product>?)null!);

            var result = await _controller.AddToCart(product.Id);

            result.Should().BeOfType<OkObjectResult>();
            _mockSessionWrapper.Verify(s => s.SetObject(SessionKeyCart, It.IsAny<List<Product>>()), Times.Once);
        }
    }
}
