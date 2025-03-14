using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using API.Configurations.Session;
using API.Hubs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories.Product;


namespace API.Controllers
{
    /// <summary>
    /// Handles operations related to the shopping cart.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CartController : ControllerBase
    {
        private const string SessionKeyCart = "Cart";

        private readonly ProductRepository _productRepository;

        private readonly IHubContext<ProductHub> _productHubContext;
        private readonly IHubContext<CartHub> _cartHubContext;

        private readonly ILogger<CartController> _logger;


        private readonly ISessionWrapper _sessionWrapper;

        public CartController(ProductRepository productRepository, IHubContext<ProductHub> productHubContext, 
            IHubContext<CartHub> cartHubContext, ILogger<CartController> logger, ISessionWrapper sessionWrapper)
        {
            _productRepository = productRepository;
            _productHubContext = productHubContext;
            _cartHubContext = cartHubContext;
            _logger = logger;
            _sessionWrapper = sessionWrapper;
        }


        /// <summary>
        /// Adds a product to the cart.
        /// </summary>
        /// <param name="productId">The ID of the product to add.</param>
        /// <returns>Updated cart contents.</returns>
        /// <response code="200">Product added successfully.</response>
        /// <response code="400">Product not found or invalid data.</response>
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> AddToCart(int productId)
        {
            try
            {
                _logger.LogInformation("Attempting to add product with ID {ProductId} to cart", productId);

                var cart = _sessionWrapper.GetObject<List<Product>>(SessionKeyCart) ?? new List<Product>();

                var product = await _productRepository.FindAsync(productId)!;

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", productId);

                    return BadRequest("Invalid product data.");
                }

                if (product.Quantity == 0)
                {
                    _logger.LogWarning("Product with ID {ProductId} is out of stock.", productId);
                    return BadRequest("Product is out of stock.");
                }

                _logger.LogInformation("Current cart before adding: {Cart}", JsonConvert.SerializeObject(cart));

                var existingProduct = cart.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    cart.Add(new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = 1,
                        ImageSource = product.ImageSource
                    });
                }

                product.Quantity--;

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                _sessionWrapper.SetObject(SessionKeyCart, cart);

                _logger.LogInformation("Cart after adding: {Cart}", JsonConvert.SerializeObject(cart));


                await _productHubContext.Clients.All.SendAsync("ProductUpdated", productId);
                await _cartHubContext.Clients.All.SendAsync("CartUpdated");

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product to cart.");
                return BadRequest("Invalid product data.");
            }
        }

        /// <summary>
        /// Retrieves the current contents of the cart.
        /// </summary>
        /// <returns>List of products in the cart.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        public ActionResult<IEnumerable<Product>> GetCart()
        {
            _logger.LogInformation("Retrieving cart contents from session.");

            var cartJson = _sessionWrapper.GetString(SessionKeyCart);
            if (string.IsNullOrEmpty(cartJson))
            {
                _logger.LogWarning("Session cart is EMPTY.");
                return Ok(new List<Product>());
            }

            var cart = JsonConvert.DeserializeObject<List<Product>>(cartJson);
            _logger.LogInformation("Cart retrieved from session: {Cart}", cart);

            return Ok(cart);
        }

        /// <summary>
        /// Calculates the total amount of the products in the cart.
        /// </summary>
        /// <returns>The total price of all items in the cart.</returns>
        [HttpGet("totalAmount")]
        [ProducesResponseType(typeof(decimal), 200)]
        public ActionResult<decimal> GetCartTotalAmount()
        {
            _logger.LogInformation("Calculating total amount for the cart.");

            var cart = _sessionWrapper.GetObject<List<Product>>(SessionKeyCart) ?? new List<Product>();

            var totalAmount = CalculateTotalAmount(cart);

            _logger.LogInformation("Total cart amount: {TotalAmount}", totalAmount);

            return Ok(totalAmount);
        }


        /// <summary>
        /// Clears all items from the cart.
        /// </summary>
        /// <returns>A confirmation message.</returns>
        [HttpGet("clear")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> ClearCart()
        {
            _logger.LogInformation("Attempting to clear the cart.");

            var cartJson = _sessionWrapper.GetString(SessionKeyCart);

            if (string.IsNullOrEmpty(cartJson))
            {
                _logger.LogWarning("Cart is already empty.");
                return Ok("Cart is already empty.");
            }

            var cart = JsonConvert.DeserializeObject<List<Product>>(cartJson);

            foreach (var item in cart!)
            {
                var product = await _productRepository.FindAsync(item.Id)!;
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    _productRepository.Update(product);
                }
            }

            await _productRepository.SaveChangesAsync();

            var emptyCart = JsonConvert.SerializeObject(new List<Product>());

            _sessionWrapper.Remove(SessionKeyCart);

            _logger.LogInformation("Cart cleared successfully.");

            await _cartHubContext.Clients.All.SendAsync("CartUpdated");

            return Ok("Cart cleared.");
        }

        /// <summary>
        /// Calculates the total amount of the items in the cart.
        /// </summary>
        /// <param name="products">List of products in the cart.</param>
        /// <returns>The total price.</returns>
        private decimal CalculateTotalAmount(IEnumerable<Product>? products)
        {
            if (products == null) return 0;

            decimal total = 0;
            foreach (var item in products)
            {
                total += item.Price * item.Quantity;
            }
            return total;
        }
    }
}
