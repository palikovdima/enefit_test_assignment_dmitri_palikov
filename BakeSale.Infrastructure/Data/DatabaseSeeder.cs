using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using System.Formats.Asn1;
using CsvHelper.Configuration;
using System.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain.Models;
using Domain.Entities;
using Domain.Service.Environment;

namespace Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly EnvironmentSettingsService _envSettingsService;


        public DatabaseSeeder(ILogger<DatabaseSeeder> logger, EnvironmentSettingsService envSettingsService)
        {
            _logger = logger;
            _envSettingsService = envSettingsService;
        }

        public async Task SeedAsync(AppDbContext context)
        {
            // Test purpose only. Deletes all records before seeding
            _logger.LogInformation("Starting database seeding...");

            _logger.LogInformation("Deleting existing records from TransactionProducts, Products, and Transactions tables.");
            await context.TransactionProducts.ExecuteDeleteAsync();
            await context.Products.ExecuteDeleteAsync();
            await context.Transactions.ExecuteDeleteAsync();


            //string basePath = AppContext.BaseDirectory;
            //string apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "BakeSale.API"));
            //string ediblesPath = Path.Combine(apiPath, "Config", "CSV", "Edibles.csv");
            //string donatedItemsPath = Path.Combine(apiPath, "Config", "CSV", "DonatedItems.csv");

            //string basePath = AppContext.BaseDirectory;
            //string apiPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..", "BakeSale.API"));
            //string ediblesPath = Path.GetFullPath("/app/Config/CSV/Edibles.csv");
            //string donatedItemsPath = Path.GetFullPath("/app/Config/CSV/DonatedItems.csv");

            string ediblesPath = Path.GetFullPath($"{_envSettingsService.GetCSVPath()}/Edibles.csv");
            string donatedItemsPath = Path.GetFullPath($"{_envSettingsService.GetCSVPath()}/DonatedItems.csv");

            _logger.LogInformation("ediblesPath path in DatabaseSeeder: {ediblesPath}", ediblesPath);
            _logger.LogInformation("donatedItemsPath path in DatabaseSeeder: {donatedItemsPath}", donatedItemsPath);



            var edibles = LoadProductsFromCsv(ediblesPath, true);

            if (!edibles.Any())
            {
                _logger.LogWarning("No edibles data found in CSV.");
                return;
            }

            var donatedItems = LoadProductsFromCsv(donatedItemsPath, false);

            if (!donatedItems.Any())
            {
                _logger.LogWarning("No donated items data found in CSV.");
                return;
            }

            var products = edibles.Concat(donatedItems);

            _logger.LogInformation("Adding products to the database.");

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }

        private List<Product> LoadProductsFromCsv(string relativePath, bool isEdible)
        {
            string basePath = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(basePath, relativePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogError("CSV file not found: {FilePath}", fullPath);
                return new List<Product>();
            }

            _logger.LogInformation("Loading data from CSV file: {FilePath}", fullPath);

            var conf = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };
            try
            {
                using var reader = new StreamReader(fullPath);
                using var csv = new CsvReader(reader, conf);
                var records = csv.GetRecords<Product>().ToList();

                _logger.LogInformation("{RecordCount} records loaded from CSV.", records.Count);

                foreach (var product in records)
                {
                    //// ? $"http://localhost:7190/app/images/Products/Edibles/{product.ImageSource}"
                    //// : $"http://localhost:7190/app/images/Products/DonatedItems/{product.ImageSource}";
                    product.ImageSource = isEdible
                        ? $"{_envSettingsService.GetStaticFilesOnBackendPath()}/Products/Edibles/{product.ImageSource}"
                        : $"{_envSettingsService.GetStaticFilesOnBackendPath()}/Products/DonatedItems/{product.ImageSource}";
                }

                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading CSV file: {FilePath}", fullPath);
                return new List<Product>();
            }
        }

    }
}
