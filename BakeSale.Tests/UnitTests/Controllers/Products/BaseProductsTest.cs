using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories.Product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Tests.Mocks;

namespace Tests.UnitTests.Controllers.Products
{
    public abstract class BaseProductsTest
    {
        protected readonly Mock<ILogger<ProductsController>> _loggerMock;
        protected readonly Mock<ProductRepository> _productRepositoryMock;
        protected readonly ProductsController _controller;

        protected BaseProductsTest()
        {
            _loggerMock = new Mock<ILogger<ProductsController>>();

            _productRepositoryMock = Mocks.MockRepository.GetProductRepository(GetType());

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
