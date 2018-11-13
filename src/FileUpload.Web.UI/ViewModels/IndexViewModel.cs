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

        public UploadViewModel Upload { get; }
        public BrowseViewModel Browser { get; }

        public IndexViewModel(string appVersion, UploadViewModel upload, BrowseViewModel browser = null)
        {
            Ensure.NotNull(appVersion, "appVersion");
            Ensure.NotNull(upload, "upload");
            AppVersion = appVersion;
            Upload = upload;
            Browser = browser;
        }
    }
}
