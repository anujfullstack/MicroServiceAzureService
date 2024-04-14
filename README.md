# MicroService-Azure-Service

## Overview

MicroServiceBlobService is a .NET library designed to facilitate operations related to uploading, downloading, and managing blobs (files) in Azure Blob Storage. It provides functionalities such as uploading/downsizing images, validating headers, obtaining SAS (Shared Access Signature) URLs for blob access, and more.

## Features

- **UploadBlobAsync**: Uploads a blob (file) to Azure Blob Storage with the option to downsize images.
- **DownloadBlobAsync**: Downloads a blob (file) from Azure Blob Storage.
- **DeleteBlobAsync**: Deletes a blob (file) from Azure Blob Storage.
- **GetDownloadUrl**: Generates a SAS (Shared Access Signature) URL for downloading a blob (file).
- **DownloadBlobWithSasToken**: Downloads a blob (file) using a SAS (Shared Access Signature) token.
- **ValidateHeaders**: Validates the headers of a blob (file) upload request.
- **IsImage**: Checks whether a file is an image based on its file extension or MIME type.
- **SetBlobMetaData**: Sets metadata for an uploaded blob (file) in Azure Blob Storage.

## Installation

To use MicroServiceBlobService in your project, you can install it via NuGet Package Manager:

```bash
Install-Package MicroServiceBlobService
```


Here's a sample structure for your README file:

---

# MicroServiceBlobService

## Introduction
This microservice provides functionalities to upload, download, and manage files in Azure Blob Storage. It offers support for creating folders dynamically, generating unique file names, generating URLs for uploaded files, and downsizing images if needed.

## Features
1. **Dynamic Folder Creation**: The service automatically creates a folder with the agency ID as its name within the specified container.
2. **Unique File Naming**: Each file uploaded is given a new GUID to ensure uniqueness.
3. **Image Handling**:
   - Original Image URL: Provided for all image files (JPEG, BMP, PNG, etc.).
   - Downgraded Image URL: If an uploaded file is an image, a downsized version is created, and its URL is provided alongside the original.
4. **Unit Tests**: Over 20 unit test cases cover various scenarios, including different file sizes (from 50 MB to 250 MB) and error handling scenarios.
5. **Exception Handling**: Proper exception handling ensures robustness against errors.
6. **Azure Storage Credentials**: Before starting, ensure that you have Azure Storage connection string, account name, and account key configured.

## Usage
To use this microservice, follow these steps:
1. **Set Up Azure Storage Credentials**: Make sure you have the Azure Storage connection string, account name, and account key ready.
2. **Instantiate BlobService Client**: Create an instance of the BlobService class by passing the necessary Azure Storage credentials.
3. **Upload Files**: Use the UploadBlobAsync method to upload files to Azure Blob Storage. The method automatically creates the required folder structure and generates unique file names.
4. **Download Files**: Utilize the DownloadBlobAsync method to download files from Azure Blob Storage.
5. **Delete Files**: Use the DeleteBlobAsync method to delete files from Azure Blob Storage.

## Example
```csharp
// Instantiate BlobService client
var blobService = new BlobService(storageConnectionString, storageAccountName, storageAccountKey);

// Upload file
var blobUploadRequest = new BlobUploadRequest
{
    ContainerName = "example-container",
    FolderName = "agency-id",
    FileName = "example.txt",
    Content = // Provide file content stream,
    ContentType = "text/plain"
};
var uploadResult = await blobService.UploadBlobAsync(blobUploadRequest);

// Download file
var downloadRequest = new BlobUploadRequest
{
    ContainerName = "example-container",
    BlobName = "agency-id/example.txt"
};
var fileStream = await blobService.DownloadBlobAsync(downloadRequest);

// Delete file
var deleteRequest = new BlobUploadRequest
{
    ContainerName = "example-container",
    BlobName = "agency-id/example.txt"
};
var isDeleted = await blobService.DeleteBlobAsync(deleteRequest);
```

## Unit Tests
The unit tests cover various scenarios, including:
--Uploading of All given type of file (".ppt", ".pptx", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".mp4", ".avi", ".mov", ".pdf", ".csv")
- Uploading files with different sizes (50 Mb to 300 Mb) and content types ("application/json","text/plain","text/xml","text/csv","application/pdf","image/gif","image/jpeg","image/png","application/xml","application/octet-stream","application/vnd.ms-powerpoint","application/vnd.openxmlformats-officedocument.presentationml.presentation","application/msword","application/vnd.openxmlformats-officedocument.wordprocessingml.document","application/vnd.ms-excel","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","image/bmp","video/mp4","video/x-msvideo","video/quicktime") 
- Handling exceptions for invalid inputs and errors during file operations.
- Ensuring proper functionality of URL generation and folder creation.

### Upload a Blob
```csharp
var blobService = new BlobService("connectionString", "accountName", "accountKey");
var blobUploadRequest = new BlobUploadRequest
{
    ContainerName = "containerName",
    FolderName = "folderName",
    FileName = "fileName.jpg",
    ContentType = "image/jpeg",
    Content = fileStream // Provide the stream of the file to be uploaded
};
var uploadResult = await blobService.UploadBlobAsync(blobUploadRequest);
```

### Download a Blob

```csharp
var blobService = new BlobService("connectionString", "accountName", "accountKey");
var blobDownloadRequest = new BlobUploadRequest
{
    ContainerName = "containerName",
    BlobName = "blobName.jpg"
};
var blobStream = await blobService.DownloadBlobAsync(blobDownloadRequest);
```

### Generate SAS URL for Blob

```csharp
var blobService = new BlobService("connectionString", "accountName", "accountKey");
var sasUrl = blobService.GetDownloadUrl("containerName", "blobName.jpg");
```

### Finaloutput 
![Microservice-Azure](https://github.com/AnujTheDev/MicroServiceAzureService/assets/141553432/f49968a4-3717-49b9-a7b6-f41b0099633f)

### Azure Storage 
![Screenshot_6](https://github.com/AnujTheDev/MicroServiceAzureService/assets/141553432/ccee3676-3e3d-48c4-acff-999c6118d15c)
![Screenshot_1](https://github.com/AnujTheDev/MicroServiceAzureService/assets/141553432/6a135197-18d7-467d-a985-979385d65d67)


