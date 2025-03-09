using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Service.Environment
{
    public class EnvironmentSettingsService
    {
        private readonly EnvironmentSettings _envSettings;

        public EnvironmentSettingsService(EnvironmentSettings envSettings)
        {
            _envSettings = envSettings;
        }

        public string GetFrontendUrl() => _envSettings!.FrontendUrl!;
        public string GetBackendUrl() => _envSettings!.BackendUrl!;
        public string GetImagePath() => _envSettings!.ImagePath!;
        public string GetCSVPath() => _envSettings!.CSVPath!;
        public string GetStaticFilesPath() => _envSettings!.StaticFilesPath!;
        public string GetCurrenciesPath() => _envSettings!.CurrenciesPath!;
        public string GetCurrencyImagesPath() => _envSettings!.CurrencyImagesPath!;
        public string GetStaticFilesOnBackendPath() => $"{_envSettings!.BackendUrl}{_envSettings!.StaticFilesPath}";

    }

}
