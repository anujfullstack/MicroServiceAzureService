using Microsoft.AspNetCore.Mvc;

namespace MicroServiceAzureService.Helpers
{
    public class ConfigrationInstances
    {
        public IConfiguration Configuration { get; set; }
        public KeyVaultHelper KeyVaultHelper { get; set; }

        public ConfigrationInstances(IConfiguration configuration)
        {
            Configuration = configuration;
            KeyVaultHelper = new KeyVaultHelper(configuration);

        }
        public string StorageConnectionString()
        {
            return Convert.ToString(KeyVaultHelper.GetSecretAsync(Configuration["StorageConnectionString"]));
        }
        public string StorageAccountKey()
        {
            return Convert.ToString(KeyVaultHelper.GetSecretAsync(Configuration["StorageAccountKey"]));
        }
        public string StorageAccountName => Convert.ToString(Configuration["StorageAccountName"]);
        public string TenantId => Convert.ToString(Configuration["tenantId"]);
        public string KeyVaultName => Convert.ToString(Configuration["keyVaultName"]);
        public string ClientId => Convert.ToString(Configuration["clientId"]);
        public string ClientSecret => Convert.ToString(Configuration["clientSecret"]);

    }
}
