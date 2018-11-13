using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class UploadViewModel
    {
        public string Url { get; }

        public UploadViewModel(string url)
        {
            Ensure.NotNull(url, "url");
            Url = url;
        }
    }
}
