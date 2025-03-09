using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Tests.Mocks;

namespace Tests.UnitTests.Checkout.CalculateChange
{
    public class CalculateChangeBackAmountTests : BaseCheckoutTest
    {
        [Fact]
        public void CalculateChangeBackAmount_ShouldReturnBadRequest_WhenAmountIsNotEnough()
        {
            decimal totalAmount = 10.00m;
            decimal paidAmount = 5.00m;

            var result = _controller.CalculateChangeBackAmount(totalAmount, paidAmount);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("The amount inserted wasn't enough.", badRequestResult.Value);
        }

        [Fact]
        public void CalculateChangeBackAmount_ShouldReturnOk_WhenAmountIsEnough()
        {
            decimal totalAmount = 10.00m;
            decimal paidAmount = 15.00m;

            var result = _controller.CalculateChangeBackAmount(totalAmount, paidAmount);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(5.00m, okResult.Value);
        }
    }
}
