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
        private readonly string dateTimeFormat;

        public string DeleteUrl { get; }
        public string ReloadUrl { get; }

        public IReadOnlyList<FileModel> Files { get; }
        public bool IsDownloadEnabled { get; }
        public bool IsDeleteEnabled { get; }

        public BrowseViewModel(IReadOnlyList<FileModel> files, string downloadUrl, string reloadUrl, string deleteUrl, bool isDownloadEnabled, bool isDeleteEnabled, string dateTimeFormat)
        {
            Ensure.NotNull(files, "files");
            Ensure.NotNull(downloadUrl, "downloadUrl");
            Ensure.NotNull(deleteUrl, "deleteUrl");
            Ensure.NotNull(reloadUrl, "reloadUrl");
            Files = files;
            IsDownloadEnabled = isDownloadEnabled;
            IsDeleteEnabled = isDeleteEnabled;
            ReloadUrl = reloadUrl;
            DeleteUrl = deleteUrl;
            this.downloadUrl = downloadUrl;
            this.dateTimeFormat = dateTimeFormat;
        }

        public string GetFileUrl(FileModel file)
        {
            if (downloadUrl.EndsWith('/'))
                return downloadUrl + file.Name;
            else
                return downloadUrl + '/' + file.Name;
        }

        public string GetDeleteUrl() => DeleteUrl;

        public string Format(DateTime dateTime)
        {
            if (String.IsNullOrEmpty(dateTimeFormat))
                return dateTime.ToString();

            return dateTime.ToString(dateTimeFormat);
        }

        public string FormatSize(long size) => Math.Max(1, Math.Round((decimal)size / 1000, MidpointRounding.AwayFromZero)).ToString("N0");
    }
}
