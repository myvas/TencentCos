using System.Collections.Generic;

namespace CosStore
{
    public class UploadOptions
    {
        public long MaxLength { get; set; }
        public List<string> SupportedExtensions { get; } = new List<string>();

        public string StorageUri { get;set;}

        public string StoragePath { get; set; }
        public bool IsOverrideEnabled { get; set; }
    }
}