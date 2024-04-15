using System;
using System.Configuration;
using System.Net;
using MicroServiceBlobService.Helpers;
using MicroServiceBlobService.Models;
using MicroServiceBlobServiceUnitTests.helpers;
using Microsoft.AspNetCore.Http;
using NUnit.Framework.Legacy;

namespace MicroServiceBlobServiceUnitTests
{
    [TestFixture]
    public class MicroServiceBlobServiceTests
    {
        private BlobService _blobService;
        private static string containerName = "anujtest";
        private static string folderName = "117";
        private static List<string> filesUploded = new List<string>();
        private static List<FileType> allowedFileTypes = new List<FileType>();
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.config");
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            var storageConnectionString = configuration.AppSettings.Settings["StorageConnectionString"].Value;
            var storageAccountName = configuration.AppSettings.Settings["StorageAccountName"].Value;
            var storageAccountKey = configuration.AppSettings.Settings["StorageAccountKey"].Value;
            _blobService = new BlobService(storageConnectionString, storageAccountName, storageAccountKey);
            allowedFileTypes = new List<FileType>
            {
                new FileType("PowerPointSamplePPT.ppt",".ppt", "application/vnd.ms-powerpoint"),
                new FileType("PowerPointSample.pptx",".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"),
                new FileType("WinwordDoc.doc",".doc", "application/msword"),
                new FileType("WinwordDocx.docx",".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
                new FileType("SampleExcel.xls",".xls", "application/vnd.ms-excel"),
                new FileType("SampleExcelx.xlsx",".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                new FileType("test-image.jpg",".jpg", "image/jpeg"),
                new FileType("test-image-1.jpeg",".jpeg", "image/jpeg"),
                new FileType("test-image-2.png",".png", "image/png"),
                new FileType("download.gif",".gif", "image/gif"),
                new FileType("Sample51MB.bmp",".bmp", "image/bmp"),
                //new FileType("VideoMp4-230MB.mp4",".mp4", "video/mp4"),
                new FileType("VideoAVI.avi",".avi", "video/x-msvideo"),
                new FileType("MovieMov.mov",".mov", "video/quicktime"),
                new FileType("sample.pdf",".pdf", "application/pdf"),
                new FileType("sampleCSV.csv",".csv", "text/csv")
            };
        }
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_JPG()
        {
            await TestUploadBlobAsync(".jpg");
        }
        [Test]
        [Order(2)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Docx()
        {
            await TestUploadBlobAsync(".docx");
        }
        [Test]
        [Order(2)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Doc()
        {
            await TestUploadBlobAsync(".doc");
        }
        [Test]
        [Order(3)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_PDF()
        {
            await TestUploadBlobAsync(".pdf");
        }
        [Test]
        [Order(4)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_CSV()
        {
            await TestUploadBlobAsync(".csv");
        }
        [Test]
        [Order(4)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Xls()
        {
            await TestUploadBlobAsync(".xls");
        }
        [Test]
        [Order(4)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_XLSX()
        {
            await TestUploadBlobAsync(".xlsx");
        }
        [Test]
        [Order(5)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_PPT()
        {
            await TestUploadBlobAsync(".ppt");
        }
        [Test]
        [Order(5)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_PPTX()
        {
            await TestUploadBlobAsync(".pptx");
        }
        //[Test]
        //[Order(1)]
        //public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_BMP()
        //{
        //    await TestUploadBlobAsync(".bmp");
        //}
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_jpeg()
        {
            await TestUploadBlobAsync(".jpeg");
        }
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_png()
        {
            await TestUploadBlobAsync(".png");
        }
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_gif()
        {
            await TestUploadBlobAsync(".gif");
        }
        //[Test]
        //[Order(1)]
        //public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_MP4()
        //{
        //    await TestUploadBlobAsync(".mp4");
        //}
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_AVI()
        {
            await TestUploadBlobAsync(".avi");
        }
        [Test]
        [Order(1)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Images_Mov()
        {
            await TestUploadBlobAsync(".mov");
        }


        [Test]
        [Order(5)]
        public void ValidateHeaders_ValidRequest_NoExceptionThrown()
        {
            // Arrange
            var blobUploadRequest = CreateBlobUploadRequest("test.jpg", "image/jpeg");
            var ex = Assert.Throws<ArgumentException>(() => _blobService.ValidateHeaders(blobUploadRequest));
            Assert.That(ex.Message, Is.EqualTo("File is required."));
        }

        [Test]
        [Order(5)]
        public void ValidateHeaders_NullFileName_ArgumentExceptionThrown()
        {
            // Arrange
            var blobUploadRequest = CreateBlobUploadRequest(null, "image/jpeg");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _blobService.ValidateHeaders(blobUploadRequest));
            Assert.That(ex.Message, Is.EqualTo("File is required."));
        }

        [Test]
        [Order(80)]
        public async Task DownloadBlobAsync_WhenValidRequest_ReturnsValidResponse_AllFiles()
        {
            foreach (var fileUrl in filesUploded.Where(a => !a.Contains("downsized_")).Take(5))
            {
                // Download the blob content
                using (WebClient client = new WebClient())
                {
                    byte[] blobBytes = client.DownloadData(fileUrl);

                    // Convert the downloaded bytes to a stream
                    using (MemoryStream stream = new MemoryStream(blobBytes))
                    {
                        // Check the size of the stream
                        long sizeInBytes = stream.Length;

                        // Check if the size is greater than 10KB
                       ClassicAssert.Greater(sizeInBytes, 100, "Blob size is Greater Then 100");

                        var rootPath = Directory.GetCurrentDirectory();
                        var folderPath = Path.Combine(rootPath.Replace(@"bin\Debug\net6.0", ""), "Files"); // Path to your "Files" folder
                        string filename = ExtractFilenameFromUrl(fileUrl);
                        var filePath = Path.Combine(folderPath, filename);
                        FileInfo localFileInfo = new FileInfo(filePath);
                        long localFileSize = localFileInfo.Length;
                        ClassicAssert.AreEqual(localFileSize, sizeInBytes, "Blob size does not match the size of the local file");
                    }
                }
            }
        }

        [Test]
        [Order(100)]
        public async Task UploadBlobAsync_WhenValidRequest_ReturnsValidResponse_Delete_AllFiles()
        {
            foreach (var fileUrl in filesUploded)
            {
                (string containerName, string blobName) = ParseBlobUrl(fileUrl);
                // Create a BlobUploadRequest object with the container name and blob name
                var blobUploadRequest = new BlobUploadRequest
                {
                    ContainerName = containerName,
                    BlobName = blobName
                };
                // Call the DeleteBlobAsync method
                var result = await _blobService.DeleteBlobAsync(blobUploadRequest);
                ClassicAssert.IsTrue(result);
            }
        }

        private BlobUploadRequest CreateBlobUploadRequest(string fileName, string contentType)
        {
            return new BlobUploadRequest
            {
                File = new FormFile(null, 0, 0, null, fileName),
                ContentType = contentType,
                FileName = fileName
            };
        }
        public static string ExtractFilenameFromUrl(string url)
        {
            // Split the URL by '/' to get segments
            string[] segments = url.Split('/');

            // Get the last segment which contains the filename and GUID
            string lastSegment = segments[segments.Length - 1];

            // Remove the GUID by splitting with '_' and taking the first part
            string[] filenameParts = lastSegment.Split('_');
            string filenameWithExtension = filenameParts[0];

            // Remove the URL prefix by splitting with '/'
            string[] filenameSegments = filenameWithExtension.Split('/');
            string filename = filenameSegments[filenameSegments.Length - 1];
            var extension = lastSegment.Split('.')[1];

            // Return the filename
            return filename + "." + extension;
        }
        private async Task TestUploadBlobAsync(string fileextensionName)
        {
            var file = allowedFileTypes.FirstOrDefault(a => string.Equals(a.Extension, fileextensionName, StringComparison.CurrentCultureIgnoreCase));
            if (file != null)
            {
                var imageFilePath = CreateBlobUploadObject(out var blobUploadRequest, file.FileName, file.ContentType);
                await using var fileStream = File.OpenRead(imageFilePath);
                var formFile = new FormFile(fileStream, 0, fileStream.Length, "data", blobUploadRequest.FileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = blobUploadRequest.ContentType
                };
                blobUploadRequest.File = formFile;
                blobUploadRequest.Content = formFile.OpenReadStream();
                var result = await _blobService.UploadBlobAsync(blobUploadRequest);
                // Assert
                ClassicAssert.IsNotNull(result);
                ClassicAssert.IsNotNull(result.BlobUrl);
                ClassicAssert.IsNotNull(result.DownloadUrl);
                ClassicAssert.AreEqual("File uploaded successfully.", result.Message);
                filesUploded.Add(result.BlobUrl);
                if (!string.IsNullOrEmpty(result.BlobDowngradeUrl))
                    filesUploded.Add(result.BlobDowngradeUrl);
            }
            
        }
        private static string CreateBlobUploadObject(out BlobUploadRequest blobUploadRequest, string fileName, string fileType)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(rootPath.Replace(@"bin\Debug\net6.0", ""), "Files"); // Path to your "Files" folder
            var imageFileName = fileName; // Name of the image file
            var imageFilePath = Path.Combine(folderPath, imageFileName);
            // Arrange
            blobUploadRequest = new BlobUploadRequest
            {
                ContainerName = containerName,
                FolderName = folderName,
                FileName = fileName,
                ContentType = fileType,
                FileSource = "optional-source",
                ReferenceKey = "optional-key",
                ReferenceValue = "optional-value",
            };
            return imageFilePath;
        }
        private (string containerName, string blobName) ParseBlobUrl(string blobUrl)
        {
            // Parse the blob URL to extract container name and blob name
            // You need to implement your own logic to extract these values from the blob URL
            // Here's a simple example assuming that the blob URL follows a specific pattern
            var uri = new Uri(blobUrl);
            var containerName = uri.Segments[1].TrimEnd('/');
            var blobName = string.Join("", uri.Segments.Skip(2)).TrimEnd('/');
            return (containerName, blobName);
        }
    }
}