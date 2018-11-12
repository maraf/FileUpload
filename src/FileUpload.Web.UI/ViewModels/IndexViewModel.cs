using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class IndexViewModel
    {
        public string AppVersion { get; }
        public string UploadUrl { get; }

        public BrowseViewModel Browser { get; }

        public IndexViewModel(string appVersion, string uploadUrl, BrowseViewModel browser = null)
        {
            Ensure.NotNull(appVersion, "appVersion");
            Ensure.NotNull(uploadUrl, "uploadUrl");
            AppVersion = appVersion;
            UploadUrl = uploadUrl;
            Browser = browser;
        }
    }
}
