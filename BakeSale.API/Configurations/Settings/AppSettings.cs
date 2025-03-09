using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace API.Configurations.Settings
{
    public class AppSettings
    {
        [Required(ErrorMessage = "Development environment settings are required.")]
        public EnvironmentSettings? Development { get; set; }

        [Required(ErrorMessage = "Production environment settings are required.")]
        public EnvironmentSettings? Production { get; set; }
    }
}
