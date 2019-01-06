using FileUpload.Models;
using FileUpload.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class Factory
    {
        private readonly FileService fileService;
        private readonly UploadSettings configuration;
        private readonly UrlBuilder urlBuilder;

        public Factory(FileService fileService, UploadSettings configuration, UrlBuilder urlBuilder)
        {
            Ensure.NotNull(fileService, "fileService");
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(urlBuilder, "urlBuilder");
            this.fileService = fileService;
            this.configuration = configuration;
            this.urlBuilder = urlBuilder;
        }

        public BrowseViewModel CreateBrowser()
        {
            IReadOnlyList<FileModel> files = fileService.FindList(configuration);
            if (files == null)
                return null;

            return new BrowseViewModel(files, urlBuilder.Index(), configuration.IsDownloadEnabled, configuration.IsDeleteEnabled);
        }

        public UploadViewModel CreateUpload()
        {
            return new UploadViewModel(urlBuilder.Upload(), urlBuilder.Download(), configuration.IsDownloadEnabled);
        }
    }
}
