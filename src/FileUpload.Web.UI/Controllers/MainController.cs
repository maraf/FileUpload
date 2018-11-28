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
        private readonly Factory factory;

        public MainController(FileService fileService, UploadSettings configuration, Factory factory)
        {
            Ensure.NotNull(fileService, "fileService");
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(factory, "factory");
            this.fileService = fileService;
            this.configuration = configuration;
            this.factory = factory;
        }

        private string GetAppVersion()
            => "v" + typeof(UploadOptions).Assembly.GetName().Version.ToString(3);

        [Route("")]
        public IActionResult Index()
        {
            return base.View(new IndexViewModel(GetAppVersion(), factory.CreateUpload(), factory.CreateBrowser()));
        }

        [Route("browse")]
        [HttpGet]
        public IActionResult Browse()
        {
            BrowseViewModel model = factory.CreateBrowser();
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Route("upload")]
        [HttpGet]
        public IActionResult Upload()
        {
            return View(factory.CreateUpload());
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

        [Route("delete")]
        public IActionResult Delete([FromServices] UrlBuilder urlBuilder, string fileName)
        {
            fileService.Delete(configuration, fileName);
            return Redirect(urlBuilder.Index());
        }

        [HttpGet("/error")]
        public IActionResult Error() => View();

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
