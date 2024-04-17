using Azure;
using Azure.Security.KeyVault.Secrets;

namespace MicroServiceAzureService.Helpers
{
    public class KeyVaultHelper
    {
        private string _keyVaultName;
        private string _clientId;
        private string _clientSecret;
        private string _tenantId;
        public static IConfiguration Configuration { get; set; }
        public KeyVaultHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tenantId = Convert.ToString(Configuration["tenantId"]);
            _keyVaultName = Convert.ToString(Configuration["keyVaultName"]);
            _clientId = Convert.ToString(Configuration["clientId"]);
            _clientSecret = Convert.ToString(Configuration["clientSecret"]);
        }

        public string GetSecretAsync(string secretName)
        {
            var credential = new Azure.Identity.ClientSecretCredential(_tenantId, _clientId, _clientSecret);
            var client = new SecretClient(new Uri($"https://{_keyVaultName}.vault.azure.net/"), credential);
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
