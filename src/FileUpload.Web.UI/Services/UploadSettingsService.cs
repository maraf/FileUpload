using FileUpload.Web.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Web.UI.Services
{
    public class UploadSettingsService
    {
        private readonly IOptions<UploadOptions> configuration;

        public UploadSettingsService(IOptions<UploadOptions> configuration)
        {
            Ensure.NotNull(configuration, "configuration");
            this.configuration = configuration;
        }

        public string FindUrlToken(RouteData routeData)
            => routeData.Values["urltoken"]?.ToString();

        public UploadSettings Find(RouteData routeData, ClaimsPrincipal principal)
        {
            string urlToken = FindUrlToken(routeData);
            return configuration.Value.Find(null);
        }
    }
}
