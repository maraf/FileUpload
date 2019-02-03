using FileUpload.Models;
using FileUpload.Services;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class Factory
    {
        private readonly FileService fileService;
        private readonly UploadSettingsService settingsService;
        private readonly UploadSettings configuration;
        private readonly UrlBuilder urlBuilder;

        public Factory(FileService fileService, UploadSettingsService settingsService, UploadSettings configuration, UrlBuilder urlBuilder)
        {
            Ensure.NotNull(fileService, "fileService");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(urlBuilder, "urlBuilder");
            this.fileService = fileService;
            this.settingsService = settingsService;
            this.configuration = configuration;
            this.urlBuilder = urlBuilder;
        }

        public BrowseViewModel CreateBrowser()
        {
            IReadOnlyList<FileModel> files = fileService.FindList(configuration);
            if (files == null)
                return null;

            return new BrowseViewModel(files, urlBuilder.Index(), urlBuilder.Browse(noLayout: true), configuration.IsDownloadEnabled, configuration.IsDeleteEnabled);
        }

        public UploadViewModel CreateUpload()
        {
            return new UploadViewModel(urlBuilder.Upload(), urlBuilder.Download(), configuration.IsDownloadEnabled);
        }

        public ProfileListViewModel CreateProfileList(ClaimsPrincipal principal)
        {
            return new ProfileListViewModel(settingsService.GetList(principal));
        }
    }
}
