using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Domain.Models;
using Domain.Service.Environment;
using Microsoft.Extensions.Logging;

namespace Domain.Service.Currency
{
    public class CurrencyImageService
    {
        private Dictionary<string, CurrencyImage>? _currencyImages;
        private readonly ILogger<CurrencyImageService> _logger;

        private readonly EnvironmentSettingsService _envSettingsService;


        public CurrencyImageService(ILogger<CurrencyImageService> logger, EnvironmentSettingsService envSettingsService)
        {
            _logger = logger;
            _envSettingsService = envSettingsService;
            LoadCurrencyImages();
        }

        private void LoadCurrencyImages()
        {
            //string basePath = AppContext.BaseDirectory;
            //string apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "BakeSale.API"));
            //string filePath = Path.Combine(apiPath, "app", "Resources", "currencyImages.json");

            //string filePath = Path.GetFullPath("/app/Resources/currencyImages.json");
            string filePath = Path.GetFullPath(_envSettingsService.GetCurrencyImagesPath());

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

                foreach (var currencyImage in _currencyImages.Values)
                {
                    foreach (var billKey in currencyImage.Bills.Keys.ToList())
                    {
                        string newBillImagePath = $"{_envSettingsService.GetStaticFilesOnBackendPath()}/Change/Bills/{currencyImage.Bills[billKey]}";
                        currencyImage.Bills[billKey] = newBillImagePath;
                    }

                    foreach (var coinKey in currencyImage.Coins.Keys.ToList())
                    {
                        string newCoinImagePath = $"{_envSettingsService.GetStaticFilesOnBackendPath()}/Change/Coins/{currencyImage.Coins[coinKey]}";
                        currencyImage.Coins[coinKey] = newCoinImagePath;
                    }
                }

                _logger.LogInformation("Successfully loaded and updated currency images.");
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
