using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class BrowseViewModel
    {
        private readonly string downloadUrl;

        public IReadOnlyList<FileViewModel> Files { get; }

        public BrowseViewModel(List<FileViewModel> fileNames, string downloadUrl)
        {
            Ensure.NotNull(fileNames, "fileNames");
            Ensure.NotNull(downloadUrl, "downloadUrl");
            Files = fileNames;
            this.downloadUrl = downloadUrl;
        }

        public string GetFileUrl(FileViewModel file)
        {
            if (downloadUrl.EndsWith('/'))
                return downloadUrl + file.Name;
            else
                return downloadUrl + '/' + file.Name;
        }
    }
}
