namespace MicroServiceBlobService.Models
{
    public class BlobUploadRequest
    {
        public string ContainerName { get; set; }
        public string FolderName { get; set; }
        private string _fileName;
        public string FileName
        {
            get { return FileNameWithGuid; }
            set { _fileName = value; }
        }
        public string FileNameWithGuid
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    throw new InvalidOperationException("FileName is not set.");
                }

                return FilenameHelper.AppendGuidToFilename(_fileName);
            }
        }
        public Stream Content { get; set; }
        public string ContentType { get; set; }
        public bool IsAsync { get; set; } = false;
        public string FileSource { get; set; } = null;
        public string ReferenceKey { get; set; } = null;
        public string ReferenceValue { get; set; } = null;
        public string BlobName { get; set; } = null;
        public IFormFile File { get; set; }
    }
    public class FileUploadModel
    {
        public string ContainerName { get; set; }
        public string FolderName { get; set; }
        private string _fileName;
        public string FileName
        {
            get { return FileNameWithGuid; }
            set { _fileName = value; }
        }
        public string FileNameWithGuid
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    throw new InvalidOperationException("FileName is not set.");
                }

                return FilenameHelper.AppendGuidToFilename(_fileName);
            }
        }
        public IFormFile File { get; set; }
        public string ContentType { get; set; }
        public bool IsAsync { get; set; }
        public string FileSource { get; set; }
        public string ReferenceKey { get; set; }
        public string ReferenceValue { get; set; }

        public BlobUploadRequest GetBlobUploadRequest()
        {
            return new BlobUploadRequest()
            {
                ContainerName = ContainerName,
                FileName = FileName,
                ContentType = ContentType,
                IsAsync = IsAsync,
                FileSource = FileSource,
                ReferenceKey = ReferenceKey,
                ReferenceValue = ReferenceValue,
                FolderName = FolderName,
                File = File,

            };

        }
    }
    public class UploadResult
    {
        public string BlobUrl { get; set; }
        public string BlobDowngradeUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string Message { get; set; }
    }

    public static class FilenameHelper
    {
        public static string AppendGuidToFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            // Get the filename without extension
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            // Get the file extension
            var fileExtension = Path.GetExtension(filename);
            // Generate a new GUID
            var guid = Guid.NewGuid().ToString();
            // Concatenate the filename without extension, GUID, and file extension
            var newFilename = $"{fileNameWithoutExtension}_{guid}{fileExtension}";
            return newFilename;
        }
    }
}
