using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceBlobServiceUnitTests.helpers
{
    public class FileType
    {
        public string FileName { get; }
        public string Extension { get; }
        public string ContentType { get; }

        public FileType(string fileName, string extension, string contentType)
        {
            FileName = fileName;
            Extension = extension;
            ContentType = contentType;
        }
    }
}
