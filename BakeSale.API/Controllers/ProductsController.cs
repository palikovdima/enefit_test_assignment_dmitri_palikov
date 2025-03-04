using BakeSale.Domain.Entities;
using BakeSale.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BakeSale.API.Controllers
{

    /// <summary>
    /// Manages product-related operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProductsController: ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IRepository<Product> productRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }


        /// <summary>
        /// Retrieves all available products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Fetching all products");

            var products = await _productRepository.AllAsync();

            if (products == null || !products.Any())
            {
                _logger.LogWarning("No products found in the database.");
                return NotFound("No products found.");
            }

            _logger.LogInformation("Successfully fetched {ProductCount} products.", products.Count());

            return Ok(products.OrderBy(p => p.Name));
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Fetching product with ID {ProductId}.", id);

            var product = await _productRepository.FindAsync(id)!;
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
                return NotFound($"Product with ID {id} not found.");
            }

            _logger.LogInformation("Successfully fetched product with ID {ProductId}.", id);
            return Ok(product);
        }
    }
}
