using FileUpload.Web.UI.Models;
using FileUpload.Web.UI.Services;
using FileUpload.Web.UI.ViewModels;
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

namespace FileUpload.Web.UI.Controllers
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

        [Route("")]
        public IActionResult Index()
        {
            string uploadUrl = null;
            string urlToken = service.FindUrlToken(RouteData);
            if (String.IsNullOrEmpty(urlToken))
                uploadUrl = Url.Action("Upload", "Upload");
            else
                uploadUrl = $"/{urlToken}/upload";

            return View(new UploadIndexViewModel() { UploadUrl = uploadUrl });
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

            fileName = $"{fileName}.{extension}";
            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar) || fileName.Contains("..") || Path.IsPathRooted(fileName))
                return NotFound();

            if (extension == null)
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

        [HttpGet("/error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
