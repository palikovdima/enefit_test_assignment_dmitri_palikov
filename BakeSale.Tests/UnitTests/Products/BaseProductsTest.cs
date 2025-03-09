using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.UnitTests.Products
{
    public abstract class BaseProductsTest
    {
        protected readonly Mock<IRepository<Product>> _productRepositoryMock;
        protected readonly Mock<ILogger<ProductsController>> _loggerMock;
        protected readonly ProductsController _controller;

        protected BaseProductsTest()
        {
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_productRepositoryMock.Object, _loggerMock.Object);
        }

        protected async Task<OkObjectResult> GetProductsAsync()
        {
            var result = await _controller.GetProducts();
            return Assert.IsType<OkObjectResult>(result.Result);
        }

        protected async Task<NotFoundObjectResult> GetProductsNotFoundAsync()
        {
            var result = await _controller.GetProducts();
            return Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        protected async Task<OkObjectResult> GetProductAsync(int id)
        {
            var result = await _controller.GetProduct(id);
            return Assert.IsType<OkObjectResult>(result.Result);
        }

        protected async Task<NotFoundObjectResult> GetProductNotFoundAsync(int id)
        {
            var result = await _controller.GetProduct(id);
            return Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}
