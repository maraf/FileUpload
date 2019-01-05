using FileUpload.Models;
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

namespace FileUpload.Services
{
    public class UploadSettingsService
    {
        private readonly IOptions<UploadOptions> configuration;

        public UploadSettingsService(IOptions<UploadOptions> configuration)
        {
            Ensure.NotNull(configuration, "configuration");
            this.configuration = configuration;
        }

        public UrlToken FindUrlToken(RouteData routeData)
        {
            string value = routeData.Values["urltoken"]?.ToString();
            if (String.IsNullOrEmpty(value))
                return null;

            return new UrlToken(value);
        }

        public UploadSettings Find(RouteData routeData, ClaimsPrincipal principal)
        {
            UploadSettings settings = FindSettings(routeData);
            if (ValidateUser(settings, principal))
                return settings;

            return null;
        }

        private UploadSettings FindSettings(RouteData routeData)
        {
            UrlToken urlToken = FindUrlToken(routeData);
            if (urlToken != null)
            {
                if (configuration.Value.Profiles.TryGetValue(urlToken.Value, out var settings))
                    return settings;

                return null;
            }

            return configuration.Value.Default;
        }

        private bool ValidateUser(UploadSettings settings, ClaimsPrincipal principal)
        {
            if (settings == null)
                return false;

            if (settings.Roles == null || settings.Roles.Count == 0)
                return true;

            if (!principal.Identity.IsAuthenticated)
                return false;

            List<string> userRoles = principal.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (userRoles == null || userRoles.Count == 0)
                return true;

            if (settings.Roles.Any(r => userRoles.Contains(r)))
                return true;

            return false;
        }
    }
}
