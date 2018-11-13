using FileUpload.Models;
using FileUpload.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Controllers.Filters
{
    public class ProfileFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        { }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            UploadSettings configuration = context.HttpContext.RequestServices.GetService<UploadSettings>();
            if (configuration == null)
                context.Result = new NotFoundResult();
        }
    }
}
