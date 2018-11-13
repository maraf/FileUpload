using FileUpload.Models;
using Microsoft.AspNetCore.Mvc;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Services
{
    public class UrlBuilder
    {
        private readonly UrlToken urlToken;
        private readonly IUrlHelper urlHelper;

        public UrlBuilder(Provider<UrlToken> urlToken, IUrlHelper urlHelper)
        {
            Ensure.NotNull(urlToken, "urlToken");
            Ensure.NotNull(urlHelper, "urlHelper");
            this.urlToken = urlToken.Optional;
            this.urlHelper = urlHelper;
        }

        // Because with want URLs both with and wihout UrlToken and ASP.NET can't generate such (it places UrlToken as a QueryString parameter).
        private string GetActionUrl(string actionName)
        {
            string uploadUrl = null;
            if (urlToken == null)
                uploadUrl = urlHelper.Action(actionName, "Main");
            else if (actionName != "index")
                uploadUrl = $"/{urlToken}/{actionName}";
            else
                uploadUrl = $"/{urlToken}";

            return uploadUrl;
        }

        public string Index() => GetActionUrl("index");

        public string Download() => GetActionUrl("index");

        public string Upload() => GetActionUrl("upload");
    }
}
