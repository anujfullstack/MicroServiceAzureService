using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using MicroServiceAzureService.Models;
using Microsoft.Extensions.Logging;

namespace MicroServiceAzureService.Helpers
{
    /// <summary>
    /// Helper class for interacting with Azure Blob Storage.
    /// </summary>
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _storageAccountName;
        private readonly string _storageAccountKey;
        private readonly string _storageConnectionString;
        public ILogger Logger { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobService"/> class.
        /// </summary>
        /// <param name="storageConnectionString">The Azure Storage connection string.</param>
        /// <param name="storageAccountName">The Azure Storage account name.</param>
        /// <param name="storageAccountKey">The Azure Storage account key.</param>
        public BlobService(ILogger logger, string storageConnectionString, string storageAccountName, string storageAccountKey)
        {
            _blobServiceClient = new BlobServiceClient(storageConnectionString);
            _storageAccountName = storageAccountName;
            _storageAccountKey = storageAccountKey;
            _storageConnectionString = storageConnectionString;
            Logger = logger;
        }
        /// <summary>
        /// Uploads a blob to Azure Blob Storage.
        /// </summary>
        /// <param name="blobUploadRequest">The blob upload request containing necessary information.</param>
        /// <returns>An instance of <see cref="UploadResult"/> containing upload details.</returns>
        public async Task<UploadResult> UploadBlobAsync(BlobUploadRequest blobUploadRequest)
        {
            // Validate headers
            try
            {
                var finalResult = new UploadResult();
                ValidateHeaders(blobUploadRequest);
                // Check if the "folder" exists
                // Get a container client
                if (Logger != null)
                    Logger.LogInformation("InBlob");
                var containerClient = _blobServiceClient.GetBlobContainerClient(blobUploadRequest.ContainerName.ToLower());
                if (Logger != null)
                    Logger.LogInformation("InBlob111");
                // Create the container if it doesn't exist
                // Check if the container exists
                if (!await containerClient.ExistsAsync())
                {
                    // If the container doesn't exist, recreate it
                    var createResponse = await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
                    if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                        await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
                }
                if (Logger != null)
                    Logger.LogInformation("InBlob3");
                var blobPrefix = blobUploadRequest.FolderName + "/";
                var blobName = blobPrefix + blobUploadRequest.FileName;
                // Get a blob client
                var blobClient = containerClient.GetBlobClient(blobName);
                if (Logger != null)
                    Logger.LogInformation("InBlob4");
                var copyOfContent = blobUploadRequest.Content;
                // Upload the blob
                var response = await blobClient.UploadAsync(blobUploadRequest.Content);
                var blobUrl = blobClient.Uri.ToString();
                var fileExtension = Path.GetExtension(blobUploadRequest.File.FileName).ToLowerInvariant();
                var downGradeUrl = string.Empty;
                // Set metadata
                SetBlobMetaData(blobUploadRequest, blobClient);
                var downloadUrl = GetDownloadUrl(blobUploadRequest.ContainerName, blobUploadRequest.FileName);
                if (IsImage(fileExtension, blobUploadRequest.ContentType))
                {
                    //downGradeUrl = await UploadImageAndDownsizedVersionAsync(containerClient, blobUploadRequest, copyOfContent);
                }
                // Return URLs along with the response message
                return new UploadResult
                {
                    BlobUrl = blobUrl,
                    BlobDowngradeUrl = downGradeUrl,
                    DownloadUrl = downloadUrl,
                    Message = "File uploaded successfully."
                };
            }
            catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                return new UploadResult
                {
                    BlobUrl = String.Empty,
                    DownloadUrl = String.Empty,
                    Message = $"Blob '{blobUploadRequest.FolderName}' already exists in container '{blobUploadRequest.ContainerName}'."
                };
            }
            catch (Exception ex)
            {
                if (Logger != null)
                    Logger.LogError(ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Downloads a blob from Azure Blob Storage asynchronously.
        /// </summary>
        /// <param name="blobUploadRequest">The blob upload request containing necessary information.</param>
        /// <returns>A <see cref="Stream"/> representing the downloaded content.</returns>

        public async Task<Stream> DownloadBlobAsync(BlobUploadRequest blobUploadRequest)
        {
            // Get a container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobUploadRequest.ContainerName);

            // Get a blob client
            var blobClient = containerClient.GetBlobClient(blobUploadRequest.BlobName);

            // Download the blob
            var blobDownloadInfo = await blobClient.DownloadAsync();

            // Return the downloaded content as a stream
            return blobDownloadInfo.Value.Content;
        }
        /// <summary>
        /// Deletes a blob from Azure Blob Storage asynchronously.
        /// </summary>
        /// <param name="blobUploadRequest">The blob upload request containing necessary information.</param>
        /// <returns>A boolean indicating whether the blob was deleted successfully.</returns>

        public async Task<bool> DeleteBlobAsync(BlobUploadRequest blobUploadRequest)
        {
            // Get a container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobUploadRequest.ContainerName);

            // Get a blob client
            var blobClient = containerClient.GetBlobClient(blobUploadRequest.BlobName);

            // Delete the blob
            return await blobClient.DeleteIfExistsAsync();
        }
        /// <summary>
        /// Generates a SAS (Shared Access Signature) URL for downloading a blob.
        /// </summary>
        /// <param name="containerName">The name of the container where the blob resides.</param>
        /// <param name="blobName">The name of the blob to generate the SAS URL for.</param>
        /// <returns>The SAS URL for downloading the blob.</returns>

        private string GetDownloadUrl(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // "b" indicates a blob
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),//Let SAS token expire after 5 minutes.
                Protocol = SasProtocol.Https
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = sasBuilder.ToSasQueryParameters(GetAccountSharedKeyCredentials()).ToString();
            var sasUri = new UriBuilder(containerClient.Uri)
            {
                Query = sasToken
            };
            return sasUri.Uri.AbsoluteUri;
        }
        /// <summary>
        /// Downloads the content of a blob using the provided SAS URL with a SAS token.
        /// </summary>
        /// <param name="blobUriWithSasToken">The URI of the blob including the SAS token.</param>
        /// <returns>The byte array containing the content of the blob.</returns>
        public byte[] DownloadBlobWithSasToken(string blobUriWithSasToken)
        {
            // Create a Uri object from the blob URI string
            Uri blobUri = new Uri(blobUriWithSasToken);
            // Create a BlobClient object using the blob URI
            BlobClient blobClient = new BlobClient(blobUri, GetAccountSharedKeyCredentials());
            // Download the blob content
            BlobDownloadInfo blobDownloadInfo = blobClient.Download();
            // Read the blob content into a byte array
            byte[] blobContent = new byte[blobDownloadInfo.ContentLength];
            blobDownloadInfo.Content.Read(blobContent, 0, (int)blobDownloadInfo.ContentLength);
            // Return the byte array containing the blob content
            return blobContent;
        }
        /// <summary>
        /// Uploads a downsized version of an image to the blob storage container.
        /// </summary>
        /// <param name="containerClient">The BlobContainerClient instance.</param>
        /// <param name="blobUploadRequest">The BlobUploadRequest containing upload information.</param>
        /// <param name="originalImageStream">The original image stream to be downsized.</param>
        /// <returns>The URL of the uploaded downsized image.</returns>

        public async Task<string> UploadImageAndDownsizedVersionAsync(BlobContainerClient containerClient, BlobUploadRequest blobUploadRequest, Stream originalImageStream)
        {
            var blobName = blobUploadRequest.FolderName + "/downsized_" + blobUploadRequest.FileName;
            // Get a blob client
            var blobClient = containerClient.GetBlobClient(blobName);
            // Downsize image
            Stream downsizedImageStream = ImageResizer.ResizeImage(blobUploadRequest.Content, 400, 200, 1);
            // Upload downsized image
            originalImageStream.Position = 0;
            var response = await blobClient.UploadAsync(downsizedImageStream);
            SetBlobMetaData(blobUploadRequest, blobClient);
            return blobClient.Uri.ToString();
        }
        /// <summary>
        /// Retrieves the StorageSharedKeyCredential required for authenticating BlobClient requests.
        /// </summary>
        /// <returns>The StorageSharedKeyCredential instance.</returns>

        private StorageSharedKeyCredential GetAccountSharedKeyCredentials()
        {
            return new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);
        }
        /// <summary>
        /// Validates the headers of a BlobUploadRequest to ensure they meet specified criteria.
        /// </summary>
        /// <param name="blobUploadRequest">The BlobUploadRequest containing the headers to be validated.</param>

        public void ValidateHeaders(BlobUploadRequest blobUploadRequest)
        {
            string[] allowedExtensions = { ".ppt", ".pptx", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".mp4", ".avi", ".mov", ".pdf", ".csv" };
            // Check if Content-Type is valid
            var validContentTypes = new List<string>
            {
                "application/json",
                "text/plain",
                "text/xml",
                "text/csv",
                "application/pdf",
                "image/gif",
                "image/jpeg",
                "image/png",
                "application/xml",
                "application/octet-stream",
                "application/vnd.ms-powerpoint",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "image/bmp",
                "video/mp4",
                "video/x-msvideo",
                "video/quicktime"
            };

            // Check for potential malware (optional)
            string[] executableExtensions = { ".js", ".vbs", ".bat", ".exe", ".dll", ".cmd", ".ps1", ".jar", ".scr", ".com" };
            if (blobUploadRequest.File == null || blobUploadRequest.File.Length == 0 || string.IsNullOrEmpty(blobUploadRequest.File.FileName))
            {
                throw new ArgumentException("File is required.");
            }
            if (executableExtensions.Any(ext => blobUploadRequest.File.FileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Potential executable file detected.");
            }
            // Check file extension
            var fileExtension = Path.GetExtension(blobUploadRequest.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("File type is not supported.");
            }
            //Check if Content - Type header is provided
            if (string.IsNullOrWhiteSpace(blobUploadRequest.ContentType))
            {
                throw new ArgumentException("Content-Type header is required.");
            }
            if (!validContentTypes.Distinct().Contains(blobUploadRequest.ContentType))
            {
                throw new ArgumentException("Invalid Content-Type.");
            }
            // Check if x-file-name header is provided and non-empty
            if (string.IsNullOrWhiteSpace(blobUploadRequest.FileName))
            {
                throw new ArgumentException("x-file-name header is required and must be non-empty.");
            }
        }
        /// <summary>
        /// Checks whether a file is an image based on its file extension or MIME type.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <returns>True if the file is an image; otherwise, false.</returns>
        private static bool IsImage(string filePath, string mimeType)
        {
            // Check file extension
            string extension = Path.GetExtension(filePath);
            if (!string.IsNullOrEmpty(extension))
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                if (Array.Exists(imageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            if (!string.IsNullOrEmpty(mimeType) && mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Sets metadata for the uploaded blob based on the provided BlobUploadRequest.
        /// </summary>
        /// <param name="blobUploadRequest">The BlobUploadRequest containing metadata information.</param>
        /// <param name="blobClient">The BlobClient representing the uploaded blob.</param>
        private static void SetBlobMetaData(BlobUploadRequest blobUploadRequest, BlobClient blobClient)
        {
            var metadata = new Dictionary<string, string>
            {
                { "filesource", blobUploadRequest.FileSource },
                { "referencekey", blobUploadRequest.ReferenceKey },
                { "referencevalue", blobUploadRequest.ReferenceValue },
                { "filename", blobUploadRequest.FileName },
                { "contenttype", blobUploadRequest.ContentType },
                { "agenyid", blobUploadRequest.FolderName },
            };
            blobClient.SetMetadata(metadata);
        }
    }
}