using Microsoft.AspNetCore.Mvc;

namespace MicroServiceAzureService.Helpers
{
    public class ConfigrationInstances
    {
        public IConfiguration Configuration { get; set; }
        public ILogger Logger { get; set; }

        public KeyVaultHelper KeyVaultHelper { get; set; }

        public ConfigrationInstances(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            KeyVaultHelper = new KeyVaultHelper(configuration, logger);
            Logger = logger;
        }
        public string StorageConnectionString()
        {
            Logger.LogInformation(Convert.ToString(Configuration["StorageConnectionString"]));
            return Convert.ToString(KeyVaultHelper.GetSecretAsync(Configuration["StorageConnectionString"]));
        }
        public string StorageAccountKey()
        {
            Logger.LogInformation(Convert.ToString(Configuration["StorageAccountKey"]));
            return Convert.ToString(KeyVaultHelper.GetSecretAsync(Configuration["StorageAccountKey"]));
        }
        public string StorageAccountName => Convert.ToString(Configuration["StorageAccountName"]);
       

    }
}
