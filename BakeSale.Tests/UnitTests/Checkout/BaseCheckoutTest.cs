using BakeSale.API.Controllers;
using BakeSale.API.Hubs;
using BakeSale.Domain.Entities;
using BakeSale.Domain.Interfaces;
using BakeSale.Domain.Service;
using BakeSale.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BakeSale.Tests.UnitTests.Checkout
{
    public class BaseCheckoutTest
    {
        protected readonly Mock<IRepository<Transaction>> _transactionRepositoryMock;
        protected readonly Mock<IRepository<Product>> _productRepositoryMock;
        protected readonly Mock<IRepository<TransactionProduct>> _transactionProductRepositoryMock;
        protected readonly Mock<IHubContext<ProductHub>> _productHubContextMock;
        protected readonly Mock<IHubContext<CartHub>> _cartHubContextMock;
        protected readonly Mock<ILogger<CheckoutController>> _loggerMock;
        protected readonly Mock<ILogger<CurrencyService>> _currencyServiceLoggerMock;
        protected readonly Mock<ILogger<CurrencyImageService>> _currencyImageServiceLoggerMock;
        protected readonly CheckoutController _controller;

        public BaseCheckoutTest()
        {
            _transactionRepositoryMock = new Mock<IRepository<Transaction>>();
            _productRepositoryMock = new Mock<IRepository<Product>>();
            _transactionProductRepositoryMock = new Mock<IRepository<TransactionProduct>>();
            _productHubContextMock = new Mock<IHubContext<ProductHub>>();
            _cartHubContextMock = new Mock<IHubContext<CartHub>>();
            _loggerMock = new Mock<ILogger<CheckoutController>>();
            _currencyServiceLoggerMock = new Mock<ILogger<CurrencyService>>();
            _currencyImageServiceLoggerMock = new Mock<ILogger<CurrencyImageService>>();

            _controller = new CheckoutController(
                _transactionRepositoryMock.Object,
                _productRepositoryMock.Object,
                _transactionProductRepositoryMock.Object,
                _productHubContextMock.Object,
                _cartHubContextMock.Object,
                _loggerMock.Object,
                _currencyServiceLoggerMock.Object,
                _currencyImageServiceLoggerMock.Object
            );
            MockClientSetup.SetupMockClients(_productHubContextMock, _cartHubContextMock);
        }

        protected void VerifyClientProxies()
        {
            ClientVerificationHelper.VerifyClientProxies(_productHubContextMock, _cartHubContextMock);
        }
    }
}
