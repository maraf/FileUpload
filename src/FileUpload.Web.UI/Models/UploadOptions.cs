using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class UploadOptions
    {
        public UploadSettings Default { get; set; }
        public Dictionary<string, UploadSettings> Profiles { get; } = new Dictionary<string, UploadSettings>();
    }
}
