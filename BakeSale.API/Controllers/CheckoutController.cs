using API.Configurations.Session;
using API.Hubs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Service.Currency;
using Domain.Service.Environment;
using Infrastructure.Repositories.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Text.Json;

namespace API.Controllers
{
    /// <summary>
    /// Manages checkout and payment processing operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CheckoutController : Controller
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly ProductRepository _productRepository;
        private readonly IRepository<TransactionProduct> _transactionProductRepository;
        private readonly IHubContext<ProductHub> _productHubContext;
        private readonly IHubContext<CartHub> _cartHubContext;
        private readonly CurrencyService _currencyService;
        private readonly CurrencyImageService _currencyImageService;

        private readonly EnvironmentSettingsService _envSettingsService;

        private readonly ILogger<CheckoutController> _logger;

        private const string SessionKeyCart = "Cart";
        private ISession Session => HttpContext.Session;

        public CheckoutController(IRepository<Transaction> transactionRepository, ProductRepository productRepository,
             IRepository<TransactionProduct> transactionProductRepository, IHubContext<ProductHub> productHubContext, 
             IHubContext<CartHub> cartHubContext, ILogger<CheckoutController> logger,
             ILogger<CurrencyService> currencyServiceLogger, ILogger<CurrencyImageService> currencyImageServiceLogger,
             EnvironmentSettingsService envSettingsService)
        {
            _transactionRepository = transactionRepository;
            _productRepository = productRepository;
            _transactionProductRepository = transactionProductRepository;
            _productHubContext = productHubContext;
            _cartHubContext = cartHubContext;
            _currencyService = new CurrencyService(currencyServiceLogger, envSettingsService);
            _currencyImageService = new CurrencyImageService(currencyImageServiceLogger, envSettingsService);
            _envSettingsService = envSettingsService;
            _logger = logger;
        }

        // <summary>
        /// Validates the payment, processes the transaction, and calculates change.
        /// </summary>
        /// <param name="totalAmount">The total amount of the transaction.</param>
        /// <param name="paidAmount">The amount paid by the customer.</param>
        /// <param name="currency">The currency used for the transaction (default: EUR).</param>
        /// <returns>A response containing the change to be given back.</returns>
        [HttpGet("validatePayment")]
        public async Task<ActionResult<Change>> ValidatePayment(decimal totalAmount, decimal paidAmount, string currency = "EUR")
        {
            _logger.LogInformation("Started validating payment for amount {TotalAmount} and paid {PaidAmount}.", totalAmount, paidAmount);

            if (!IsAmountEnough(totalAmount, paidAmount))
            {
                _logger.LogWarning("The amount inserted wasn't enough. Total: {TotalAmount}, Paid: {PaidAmount}.", totalAmount, paidAmount);
                return BadRequest("The amount inserted wasn't enough.");
            }

            var cart = Session.GetObject<List<Product>>(SessionKeyCart) ?? new List<Product>();
            if (!cart.Any()) return BadRequest("Cart is empty!");

            var change = GetChange(totalAmount, paidAmount, currency);

            var transaction = new Transaction
            {
                TotalAmount = totalAmount,
                PaidAmount = paidAmount,
                ChangeJson = JsonSerializer.Serialize(change),
                Date = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            var transactionProducts = new List<TransactionProduct>();

            foreach (var soldProduct in cart)
            {
                var product = await _productRepository.FindAsync(soldProduct.Id)!;
                if (product == null) continue;

                transactionProducts.Add(new TransactionProduct
                {
                    TransactionId = transaction.Id,
                    ProductId = soldProduct.Id,
                    Quantity = soldProduct.Quantity
                });
            }

            await _transactionProductRepository.AddRangeAsync(transactionProducts);
            await _transactionProductRepository.SaveChangesAsync();

            await _productRepository.SaveChangesAsync();

            Session.Remove(SessionKeyCart);

            foreach (var soldProduct in cart)
            {
                await _productHubContext.Clients.All.SendAsync("ProductUpdated", soldProduct.Id);
            }
            await _cartHubContext.Clients.All.SendAsync("CartUpdated");

            _logger.LogInformation("Payment validated successfully, change to give: {Change}.", change);

            return Ok(change);
        }

        /// <summary>
        /// Calculates the amount of change to be returned to the customer.
        /// </summary>
        /// <param name="totalAmount">The total amount of the transaction.</param>
        /// <param name="paidAmount">The amount paid by the customer.</param>
        /// <param name="currency">The currency used for the transaction (default: EUR).</param>
        /// <returns>The calculated change amount.</returns>
        [HttpGet("calculateChangeBackAmount")]
        public ActionResult<Change> CalculateChangeBackAmount(decimal totalAmount, decimal paidAmount, string currency = "EUR")
        {
            if (!IsAmountEnough(totalAmount, paidAmount))
            {
                _logger.LogWarning("The amount inserted wasn't enough. Total: {TotalAmount}, Paid: {PaidAmount}.", totalAmount, paidAmount);
                return BadRequest("The amount inserted wasn't enough.");
            }

            var changeBackAmount = GetChangeBackAmount(totalAmount, paidAmount);
            _logger.LogInformation("Calculated change back amount successfully, change to give: {Change}.", changeBackAmount);

            return Ok(changeBackAmount);
        }

        /// <summary>
        /// Determines whether the paid amount is sufficient to cover the total amount.
        /// </summary>
        /// <param name="totalAmount">The total amount due.</param>
        /// <param name="paidAmount">The amount paid by the customer.</param>
        /// <returns>True if the paid amount is sufficient; otherwise, false.</returns>
        private bool IsAmountEnough(decimal totalAmount, decimal paidAmount)
        {
            return paidAmount >= totalAmount;
        }

        /// <summary>
        /// Calculates the amount of change to be returned.
        /// </summary>
        /// <param name="totalAmount">The total amount of the transaction.</param>
        /// <param name="paidAmount">The amount paid by the customer.</param>
        /// <returns>The computed change amount.</returns>
        private decimal GetChangeBackAmount(decimal totalAmount, decimal paidAmount)
        {
            _logger.LogInformation("GetChangeBackAmount method started with amount {TotalAmount} and paid {PaidAmount}.", totalAmount, paidAmount);

            return Math.Round(paidAmount -= totalAmount, 2);
        }

        /// <summary>
        /// Computes the change to be given back to the customer in bills and coins.
        /// </summary>
        /// <param name="totalAmount">The total amount of the transaction.</param>
        /// <param name="paidAmount">The amount paid by the customer.</param>
        /// <param name="currency">The currency used for the transaction.</param>
        /// <returns>A structured representation of the change including bills and coins.</returns>
        private Change GetChange(decimal totalAmount, decimal paidAmount, string currency)
        {
            _logger.LogInformation("GetChange method started with totalAmount: {TotalAmount}, paidAmount: {PaidAmount}, currency: {Currency}", totalAmount, paidAmount, currency);

            var currencyUnits = _currencyService.GetCurrencyUnits(currency);
            var currencyImages = _currencyImageService.GetCurrencyImages(currency);

            int totalCents = (int)Math.Round(totalAmount * 100);
            int payedCents = (int)Math.Round(paidAmount * 100);
            int changeCents = payedCents - totalCents;

            _logger.LogInformation("Calculated totalCents: {TotalCents}, payedCents: {PayedCents}, changeCents: {ChangeCents}", totalCents, payedCents, changeCents);


            Change change = new() { Bills = new Dictionary<string, ChangeItem>(), Coins = new Dictionary<string, ChangeItem>() };

            foreach (var bill in currencyUnits.Bills.OrderByDescending(b => b.Value))
            {
                int count = changeCents / bill.Value;
                if (count > 0)
                {
                    _logger.LogInformation("Adding {Count} of bill {BillName} worth {BillValue} to the change", count, bill.Key, bill.Value);
                    change.Bills[bill.Key] = new ChangeItem
                    {
                        Count = count,
                        Image = currencyImages.Bills[bill.Key]
                    };
                    changeCents %= bill.Value;
                }
            }

            foreach (var coin in currencyUnits.Coins.OrderByDescending(c => c.Value))
            {
                int count = changeCents / coin.Value;
                if (count > 0)
                {
                    _logger.LogInformation("Adding {Count} of coin {CoinName} worth {CoinValue} to the change", count, coin.Key, coin.Value);
                    change.Coins[coin.Key] = new ChangeItem
                    {
                        Count = count,
                        Image = currencyImages.Coins[coin.Key]
                    };
                    changeCents %= coin.Value;
                }
            }
            _logger.LogInformation("GetChange method completed. Final change: {Change}", change);
            foreach (var item in change.Bills)
            {
                _logger.LogInformation("Bill: {BillName}, Count: {Count}, Image: {Image}", item.Key, item.Value.Count, item.Value.Image);
            }

            return change;
        }
    }
}
