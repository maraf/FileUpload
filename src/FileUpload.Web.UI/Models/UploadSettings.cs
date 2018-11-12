using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class UploadSettings
    {
        public long MaxLength { get; set; }
        public List<string> SupportedExtensions { get; } = new List<string>();
        public string StoragePath { get; set; }
        public bool IsOverrideEnabled { get; set; }
        public bool IsDownloadEnabled { get; set; }
        public bool IsBrowserEnabled { get; set; }
    }
}
