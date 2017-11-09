using FileUpload.Web.UI.Models;
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
    public class HomeController : Controller
    {
        private readonly IOptions<UploadOptions> configuration;

        public HomeController(IOptions<UploadOptions> configuration)
        {
            Ensure.NotNull(configuration, "configuration");
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/upload")]
        public StatusCodeResult Upload(IFormFile file)
        {
            Ensure.NotNull(file, "file");

            if (file.Length > configuration.Value.MaxLength)
                return NotValidUpload();

            string extension = Path.GetExtension(file.FileName);
            if (extension == null)
                return NotValidUpload();

            extension = extension.ToLowerInvariant();
            if (!configuration.Value.SupportedExtensions.Contains(extension))
                return NotValidUpload();

            if (!Directory.Exists(configuration.Value.StoragePath))
                Directory.CreateDirectory(configuration.Value.StoragePath);

            string filePath = Path.Combine(configuration.Value.StoragePath, file.FileName);
            using (Stream fileContent = new FileStream(filePath, FileMode.OpenOrCreate))
                file.CopyTo(fileContent);

            return Ok();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
