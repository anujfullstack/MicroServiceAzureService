using MicroServiceAzureService.Helpers;
using MicroServiceAzureService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroServiceAzureService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly BlobService _blobService;
        public static IConfiguration Configuration { get; set; }

        public FileController(ILogger<FileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            
            var configrationInstances = new ConfigrationInstances(configuration);
            var connectionString = configrationInstances.StorageConnectionString();
            var storageAccountName = configrationInstances.StorageAccountName;
            var storageAccountKey = configrationInstances.StorageAccountKey();
            _blobService = new BlobService(connectionString, storageAccountName, storageAccountKey);
        }
        [HttpGet(Name = "GetFileClassVersion")]
        public string Get()
        {
            return "V1.0";
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("File is required.");
            }
            // You can add validation or other processing here
            // Call the UploadBlobAsync method
            var uploadRequest = model.GetBlobUploadRequest();
            uploadRequest.Content = model.File.OpenReadStream();
            var result = await _blobService.UploadBlobAsync(uploadRequest);
            return Ok(result);
        }
    }
}
