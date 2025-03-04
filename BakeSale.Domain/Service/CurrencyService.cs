using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BakeSale.Domain.Service
{
    public class CurrencyService
    {
        private Dictionary<string, CurrencyUnit>? _currencies;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(ILogger<CurrencyService> logger)
        {
            _logger = logger;
            LoadCurrencies();
        }

        private void LoadCurrencies()
        {
            string basePath = AppContext.BaseDirectory;
            string apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "BakeSale.API"));
            string filePath = Path.Combine(apiPath, "Resources", "currencies.json");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Currency configuration file not found at path: {FilePath}", filePath);
                throw new FileNotFoundException("Currency configuration file not found.");
            }

            try
            {
                _logger.LogInformation("Loading currency configuration from file: {FilePath}", filePath);

                string json = File.ReadAllText(filePath);
                _currencies = JsonSerializer.Deserialize<Dictionary<string, CurrencyUnit>>(json);

                if (_currencies == null)
                {
                    _logger.LogError("Failed to deserialize the currency configuration.");
                    throw new JsonException("Failed to deserialize currencies.");
                }

                _logger.LogInformation("Successfully loaded currency configuration.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading currencies.");
                throw;
            }
        }

        public CurrencyUnit GetCurrencyUnits(string currencyCode)
        {
            if (_currencies == null || !_currencies.ContainsKey(currencyCode))
            {
                _logger.LogWarning("Currency {CurrencyCode} not found in configuration.", currencyCode);
                throw new KeyNotFoundException($"Currency {currencyCode} not found in configuration.");
            }

            _logger.LogInformation("Retrieving currency configuration for {CurrencyCode}", currencyCode);
            return _currencies[currencyCode];
        }
    }

    public class CurrencyUnit
    {
        public Dictionary<string, int> Bills { get; set; } = new();
        public Dictionary<string, int> Coins { get; set; } = new();
    }
}
