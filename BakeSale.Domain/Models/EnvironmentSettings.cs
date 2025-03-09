using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class EnvironmentSettings
    {
        [JsonPropertyName("FrontendUrl")]
        [Required(ErrorMessage = "FrontendUrl is required.")]
        public string? FrontendUrl { get; set; }

        [JsonPropertyName("BackendUrl")]
        [Required(ErrorMessage = "BackendUrl is required.")]
        public string? BackendUrl { get; set; }

        [JsonPropertyName("ImagePath")]
        [Required(ErrorMessage = "ImagePath is required.")]
        public string? ImagePath { get; set; }

        [JsonPropertyName("CSVPath")]
        [Required(ErrorMessage = "CSVPath is required.")]
        public string? CSVPath { get; set; }

        [JsonPropertyName("StaticFilesPath")]
        [Required(ErrorMessage = "StaticFilesPath is required.")]
        public string? StaticFilesPath { get; set; }

        [JsonPropertyName("CurrenciesPath")]
        [Required(ErrorMessage = "CurrenciesPath is required.")]
        public string? CurrenciesPath { get; set; }

        [JsonPropertyName("CurrencyImagesPath")]
        [Required(ErrorMessage = "CurrencyImagesPath is required.")]
        public string? CurrencyImagesPath { get; set; }
    }
}
