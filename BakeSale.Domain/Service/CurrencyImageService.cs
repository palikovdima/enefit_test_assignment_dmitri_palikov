using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BakeSale.Domain.Service
{
    public class CurrencyImageService
    {
        private Dictionary<string, CurrencyImage>? _currencyImages;
        private readonly ILogger<CurrencyImageService> _logger;

        public CurrencyImageService(ILogger<CurrencyImageService> logger)
        {
            _logger = logger;
            LoadCurrencyImages();
        }

        private void LoadCurrencyImages()
        {
            string basePath = AppContext.BaseDirectory;
            string apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "BakeSale.API"));
            string filePath = Path.Combine(apiPath, "Resources", "currencyImages.json");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Currency image configuration file not found at path: {FilePath}", filePath);
                throw new FileNotFoundException("Currency image configuration file not found.");
            }

            try
            {
                _logger.LogInformation("Loading currency images from file: {FilePath}", filePath);

                string json = File.ReadAllText(filePath);
                _currencyImages = JsonSerializer.Deserialize<Dictionary<string, CurrencyImage>>(json);

                if (_currencyImages == null)
                {
                    _logger.LogError("Failed to deserialize the currency image configuration.");
                    throw new JsonException("Failed to deserialize currency images.");
                }

                _logger.LogInformation("Successfully loaded currency images.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading currency images.");
                throw;
            }
        }

        public CurrencyImage GetCurrencyImages(string currencyCode)
        {
            if (_currencyImages == null || !_currencyImages.ContainsKey(currencyCode))
            {
                _logger.LogWarning("Currency {CurrencyCode} not found in image configuration.", currencyCode);
                throw new KeyNotFoundException($"Currency {currencyCode} not found in image configuration.");
            }

            _logger.LogInformation("Retrieving images for currency {CurrencyCode}", currencyCode);

            return _currencyImages[currencyCode];
        }
    }

    public class CurrencyImage
    {
        public Dictionary<string, string> Bills { get; set; } = new();
        public Dictionary<string, string> Coins { get; set; } = new();
    }
}
