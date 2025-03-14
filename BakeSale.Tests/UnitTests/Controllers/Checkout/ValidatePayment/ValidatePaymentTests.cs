using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Configurations.Session;
using Tests.Mocks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Tests.UnitTests.Controllers.Checkout;

namespace Tests.UnitTests.Controllers.Checkout.ValidatePayment
{
    public class ValidatePaymentTests : BaseCheckoutTest
    {
        [Fact]
        public async Task ValidatePayment_ShouldReturnBadRequest_WhenAmountIsNotEnough()
        {
            decimal totalAmount = 10.00m;
            decimal paidAmount = 5.00m;

            var result = await _controller.ValidatePayment(totalAmount, paidAmount);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The amount inserted wasn't enough.", badRequestResult.Value);
        }

        [Fact]
        public async Task ValidatePayment_ShouldReturnBadRequest_WhenCartIsEmpty()
        {
            decimal totalAmount = 10.00m;
            decimal paidAmount = 15.00m;
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new FakeSession();

            var result = await _controller.ValidatePayment(totalAmount, paidAmount);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cart is empty!", badRequestResult.Value);
        }

        [Fact]
        public async Task ValidatePayment_ShouldReturnOk_WhenPaymentIsValid()
        {
            decimal totalAmount = 10.00m;
            decimal paidAmount = 15.00m;
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.00m, Quantity = 1 };

            await _productRepositoryMock.Object.AddAsync(product);
            var products = new List<Product> { product };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new FakeSession();
            _controller.ControllerContext.HttpContext.Session.SetObject("Cart", products);

            var result = await _controller.ValidatePayment(totalAmount, paidAmount);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }
    }
}
