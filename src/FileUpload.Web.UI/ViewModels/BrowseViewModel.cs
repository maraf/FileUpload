using FileUpload.Models;
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

        public string ReloadUrl { get; }

        public IReadOnlyList<FileModel> Files { get; }
        public bool IsDownloadEnabled { get; }
        public bool IsDeleteEnabled { get; }

        public BrowseViewModel(IReadOnlyList<FileModel> files, string downloadUrl, string reloadUrl, bool isDownloadEnabled, bool isDeleteEnabled)
        {
            Ensure.NotNull(files, "files");
            Ensure.NotNull(downloadUrl, "downloadUrl");
            Ensure.NotNull(reloadUrl, "reloadUrl");
            Files = files;
            IsDownloadEnabled = isDownloadEnabled;
            IsDeleteEnabled = isDeleteEnabled;
            ReloadUrl = reloadUrl;
            this.downloadUrl = downloadUrl;
        }

        public string GetFileUrl(FileModel file)
        {
            if (downloadUrl.EndsWith('/'))
                return downloadUrl + file.Name;
            else
                return downloadUrl + '/' + file.Name;
        }
    }
}
