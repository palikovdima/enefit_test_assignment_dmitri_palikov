using API.Configurations.Settings;
using API.Controllers;
using API.Hubs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Service.Currency;
using Domain.Service.Environment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Mocks;
using Xunit;

namespace Tests.UnitTests.Checkout
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
        protected readonly Mock<EnvironmentSettingsService> _environmentSettingsServiceMock;
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
            _environmentSettingsServiceMock = new Mock<EnvironmentSettingsService>(GetEnvironmentSettings());

            _controller = new CheckoutController(
                _transactionRepositoryMock.Object,
                _productRepositoryMock.Object,
                _transactionProductRepositoryMock.Object,
                _productHubContextMock.Object,
                _cartHubContextMock.Object,
                _loggerMock.Object,
                _currencyServiceLoggerMock.Object,
                _currencyImageServiceLoggerMock.Object,
                _environmentSettingsServiceMock.Object
            );
            MockClientSetup.SetupMockClients(_productHubContextMock, _cartHubContextMock);
        }

        protected void VerifyClientProxies()
        {
            ClientVerificationHelper.VerifyClientProxies(_productHubContextMock, _cartHubContextMock);
        }

        protected EnvironmentSettings GetEnvironmentSettings()
        {
            return new EnvironmentSettings
            {
                FrontendUrl = "https://localhost:62170",
                BackendUrl = "https://localhost:7190",
                ImagePath = "C:/Users/palik/source/repos/BakeSale/BakeSale.API/Config/Images",
                CSVPath = "C:/Users/palik/source/repos/BakeSale/BakeSale.API/Config/CSV",
                StaticFilesPath = "/images",
                CurrenciesPath = "C:/Users/palik/source/repos/BakeSale/BakeSale.API/Resources/currencies.json",
                CurrencyImagesPath = "C:/Users/palik/source/repos/BakeSale/BakeSale.API/Resources/currencyImages.json"
            };
        }
    }
}
