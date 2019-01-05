using FileUpload.Controllers.Filters;
using FileUpload.Models;
using FileUpload.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options => options.Filters.Add<ProfileFilter>());

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddTransient<UploadSettingsService>();
            services.AddTransient<FileService>();
            services.AddTransient<UrlBuilder>();
            services.AddTransient<ViewModels.Factory>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient(CreateUploadSettings);
            services.AddTransient(CreateUrlToken);
            services.AddTransient(CreateUrlHelper);
            services.AddTransientProvider<UrlToken>();

            services.Configure<UploadOptions>(Configuration.GetSection("Upload"));
        }

        private IUrlHelper CreateUrlHelper(IServiceProvider services)
        {
            ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
            IUrlHelperFactory factory = services.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        }

        private UploadSettings CreateUploadSettings(IServiceProvider services)
        {
            ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
            UploadSettingsService configurationService = services.GetRequiredService<UploadSettingsService>();
            UploadSettings configuration = configurationService.Find(actionContext.RouteData, actionContext.HttpContext.User);
            return configuration;
        }

        private UrlToken CreateUrlToken(IServiceProvider services)
        {
            ActionContext actionContext = services.GetRequiredService<IActionContextAccessor>().ActionContext;
            UploadSettingsService configurationService = services.GetRequiredService<UploadSettingsService>();
            return configurationService.FindUrlToken(actionContext.RouteData);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/error");

            app.UseAuthentication();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
