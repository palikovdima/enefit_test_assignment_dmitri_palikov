using API.Controllers;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.UnitTests.Products;
using Xunit;

namespace Tests.UnitTests.Products.Get
{
    public class GetProductsTests : BaseProductsTest
    {
        [Fact]
        public async Task GetProducts_ShouldReturnOk_WhenProductsExist()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10.0m, Quantity = 5 },
                new Product { Id = 2, Name = "Product2", Price = 20.0m, Quantity = 3 }
            };
            _productRepositoryMock.Setup(repo => repo.AllAsync()).ReturnsAsync(products);

            var okResult = await GetProductsAsync();

            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }

        [Fact]
        public async Task GetProducts_ShouldReturnNotFound_WhenNoProductsExist()
        {
            _productRepositoryMock.Setup(repo => repo.AllAsync()).ReturnsAsync(new List<Product>());

            var notFoundResult = await GetProductsNotFoundAsync();

            Assert.Equal("No products found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOk_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "Product1", Price = 10.0m, Quantity = 5 };
            _productRepositoryMock.Setup(repo => repo.FindAsync(1)).ReturnsAsync(product);

            var okResult = await GetProductAsync(1);

            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
        }

        [Fact]
        public async Task GetProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _productRepositoryMock.Setup(repo => repo.FindAsync(1)).ReturnsAsync((Product?)null);

            var notFoundResult = await GetProductNotFoundAsync(1);

            Assert.Equal("Product with ID 1 not found.", notFoundResult.Value);
        }
    }
}
