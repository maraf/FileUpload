using FileUpload.Controllers.Filters;
using FileUpload.Models;
using FileUpload.Services;
using FileUpload.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    [Route("")]
    [Route("{urltoken:regex([[a-zA-Z0-9]]+)}")]
    public class MainController : Controller
    {
        private readonly FileService fileService;
        private readonly UploadSettings configuration;
        private readonly UrlToken urlToken;

        public MainController(FileService fileService, UploadSettings configuration, Provider<UrlToken> urlToken)
        {
            Ensure.NotNull(fileService, "fileService");
            Ensure.NotNull(configuration, "configuration");
            this.fileService = fileService;
            this.configuration = configuration;
            this.urlToken = urlToken.Optional;
        }

        // Because with want URLs both with and wihout UrlToken and ASP.NET can't generate such (it places UrlToken as a QueryString parameter).
        private string GetActionUrl(string actionName)
        {
            string uploadUrl = null;
            if (urlToken == null)
                uploadUrl = Url.Action(actionName, "Main");
            else if (actionName != "index")
                uploadUrl = $"/{urlToken}/{actionName}";
            else
                uploadUrl = $"/{urlToken}";

            return uploadUrl;
        }

        private string GetAppVersion()
            => "v" + typeof(UploadOptions).Assembly.GetName().Version.ToString(3);

        [Route("")]
        public IActionResult Index()
        {
            string uploadUrl = GetActionUrl("upload");
            return View(new IndexViewModel(GetAppVersion(), uploadUrl, CreateBrowser()));
        }

        [Route("browser")]
        [HttpGet]
        public IActionResult Browser()
        {
            BrowseViewModel model = CreateBrowser();
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Route("upload")]
        [HttpPost]
        public async Task<StatusCodeResult> Upload(IFormFile file)
        {
            Ensure.NotNull(file, "file");
            bool isSuccess = await fileService.SaveAsync(configuration, file.FileName, file.Length, file.OpenReadStream());
            if (isSuccess)
                return Ok();

            return NotValidUpload();
        }

        [Route("{fileName}.{extension}")]
        [HttpGet]
        public IActionResult Download(string fileName, string extension)
        {
            Ensure.NotNull(fileName, "fileName");

            var content = fileService.FindContent(configuration, fileName, extension);
            if (content == null)
                return NotFound();

            return File(content.Value.Content, content.Value.ContentType);
        }

        private BrowseViewModel CreateBrowser()
        {
            IReadOnlyList<FileModel> files = fileService.FindList(configuration);
            if (files == null)
                return null;

            return new BrowseViewModel(files, GetActionUrl("index"));
        }

        [HttpGet("/error")]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
