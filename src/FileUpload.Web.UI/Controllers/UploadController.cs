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
    public class UploadController : Controller
    {
        private readonly UploadSettingsService service;

        public UploadController(UploadSettingsService service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;
        }

        // Because with want URLs both with and wihout UrlToken and ASP.NET can't generate such (it places UrlToken as a QueryString parameter).
        private string GetActionUrl(string actionName)
        {
            string uploadUrl = null;
            string urlToken = service.FindUrlToken(RouteData);
            if (String.IsNullOrEmpty(urlToken))
                uploadUrl = Url.Action(actionName, "Upload");
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

        [Route("browse")]
        [HttpGet]
        public IActionResult Browse()
        {
            BrowseViewModel model = CreateBrowser();
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Route("upload")]
        [HttpPost]
        public StatusCodeResult Upload(IFormFile file)
        {
            Ensure.NotNull(file, "file");
            UploadSettings configuration = service.Find(RouteData, User);

            if (file.Length > configuration.MaxLength)
                return NotValidUpload();

            string extension = Path.GetExtension(file.FileName);
            if (extension == null)
                return NotValidUpload();

            extension = extension.ToLowerInvariant();
            if (!configuration.SupportedExtensions.Contains(extension))
                return NotValidUpload();

            if (!Directory.Exists(configuration.StoragePath))
                Directory.CreateDirectory(configuration.StoragePath);

            string filePath = Path.Combine(configuration.StoragePath, file.FileName);
            if (System.IO.File.Exists(filePath))
            {
                if (configuration.IsOverrideEnabled)
                    System.IO.File.Delete(filePath);
                else
                    return NotValidUpload();
            }

            using (Stream fileContent = new FileStream(filePath, FileMode.OpenOrCreate))
                file.CopyTo(fileContent);

            return Ok();
        }

        [Route("{fileName}.{extension}")]
        [HttpGet]
        public IActionResult Download(string fileName, string extension)
        {
            Ensure.NotNull(fileName, "fileName");
            UploadSettings configuration = service.Find(RouteData, User);

            if (!configuration.IsDownloadEnabled)
                return Unauthorized();

            if (extension == null)
                return NotFound();

            extension = "." + extension;
            fileName = fileName + extension;
            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                return NotFound();

            extension = extension.ToLowerInvariant();
            if (!configuration.SupportedExtensions.Contains(extension))
                return NotFound();

            string filePath = Path.Combine(configuration.StoragePath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                string contentType = "application/octet-stream";
                if (extension == ".gif")
                    contentType = "image/gif";
                else if (extension == ".png")
                    contentType = "image/png";
                else if (extension == ".jpg" || extension == ".jpeg")
                    contentType = "image/jpg";

                return File(new FileStream(filePath, FileMode.Open), contentType);
            }

            return NotFound();
        }

        private BrowseViewModel CreateBrowser()
        {
            UploadSettings configuration = service.Find(RouteData, User);
            if (!configuration.IsDownloadEnabled)
                return null;

            List<FileViewModel> files = Directory
                .EnumerateFiles(configuration.StoragePath)
                .Where(f => configuration.SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => new FileViewModel(Path.GetFileName(f), new FileInfo(f).Length))
                .ToList();

            return new BrowseViewModel(files, GetActionUrl("index"));
        }

        [HttpGet("/error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
