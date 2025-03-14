using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Repositories.Product;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.UnitTests.Database.Product
{
    public class ProductTests
    {
        protected readonly Mock<ILogger<ProductTests>> _loggerMock;
        protected readonly Mock<ProductRepository> _productRepositoryMock;


        public ProductTests()
        {
            _loggerMock = new Mock<ILogger<ProductTests>>();
            _productRepositoryMock = Mocks.MockRepository.GetProductRepository(GetType());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task AddProduct_ShouldAddTheProduct_WhenProductQunatityIsPositiveNumberOrZero(int quantity)
        {
            var product = new Domain.Entities.Product { Name = "Product1", Price = 10.0m, Quantity = quantity };
            _productRepositoryMock.Setup(repo => repo.Add(product));
            _productRepositoryMock.Setup(repo => repo.AllAsync()).ReturnsAsync(new List<Domain.Entities.Product> { product }); ;
            Assert.Equal(1, (await _productRepositoryMock.Object.AllAsync()).Count());
        }


        [Fact]
        public void AddProduct_ShouldThrowException_WhenProductQunatityIsNegativeNumber()
        {
            int quantity = -1;
            var product = new Domain.Entities.Product { Name = "Product1", Price = 10.0m, Quantity = quantity };
            _productRepositoryMock.Setup(repo => repo.Add(product)).Throws(new InvalidEntityException());
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1.0)]
        public async Task AddProduct_ShouldAddTheProduct_WhenProductPriceIsPositiveNumberOrZero(decimal price)
        {
            var product = new Domain.Entities.Product { Name = "Product1", Price = price, Quantity = 10 };
            _productRepositoryMock.Setup(repo => repo.Add(product));
            _productRepositoryMock.Setup(repo => repo.AllAsync()).ReturnsAsync(new List<Domain.Entities.Product> { product }); ;
            Assert.Equal(1, (await _productRepositoryMock.Object.AllAsync()).Count());
        }


        [Fact]
        public void AddProduct_ShouldThrowException_WhenProductPriceIsNegativeNumber()
        {
            decimal price = -1.0m;
            var product = new Domain.Entities.Product { Name = "Product1", Price = price, Quantity = 10 };
            _productRepositoryMock.Setup(repo => repo.Add(product)).Throws(new InvalidEntityException());
        }
    }
}
