
using Microsoft.Extensions.Configuration;

namespace MyCred_Core.Utilities
{
    public class ConfigReader
    {
        private readonly IConfiguration _config;

        public ConfigReader()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)  // Ensure to use BaseDirectory without parentheses
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Modify this method to handle nested configuration
        public string GetConfigValue(string key)
        {
            // Assuming your config is nested under 'WebConfig'
            var value = _config.GetSection("WebConfig")[key];  // Access the WebConfig section
            return value ?? throw new Exception($"Key '{key}' not found in appsettings.json.");
        }
    }
}
