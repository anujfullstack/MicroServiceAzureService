using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace MicroServiceAzureService.Helpers
{
    public class KeyVaultHelper
    {
        private string _keyVaultName;
        public static IConfiguration Configuration { get; set; }
        public ILogger Logger { get; set; }

        public KeyVaultHelper(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            //logger.LogInformation("KeyVaultClass---"+Convert.ToString(Configuration["keyVaultName"]));
            _keyVaultName = Convert.ToString(Configuration["keyVaultName"]);
            Logger = logger;
        }

        public string GetSecretAsync(string secretName)
        {
            //Logger.LogInformation("KeyVaultClass---" + Convert.ToString(Configuration["StorageConnectionString"]));
            var keyVaultUrl = Convert.ToString(Configuration["KeyVaultUrl"]);
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            try
            {
                KeyVaultSecret secret = client.GetSecret(secretName);
                return secret.Value;
            }
            catch (RequestFailedException ex)
            {
                // Handle the exception
                Console.WriteLine($"Error retrieving secret '{secretName}': {ex.Message}");
                return null;
            }
        }
    }
}
