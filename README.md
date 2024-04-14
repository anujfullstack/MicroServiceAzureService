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

## Usage

Here are some examples demonstrating the usage of MicroServiceBlobService:

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


