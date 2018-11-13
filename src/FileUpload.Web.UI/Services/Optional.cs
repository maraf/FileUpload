using FileUpload.Services;
using Microsoft.Extensions.DependencyInjection;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Services
{
    public class Provider<T>
    {
        private readonly IServiceProvider services;

        public Provider(IServiceProvider services)
        {
            Ensure.NotNull(services, "services");
            this.services = services;
        }

        public T Optional => services.GetService<T>();
        public T Required => services.GetRequiredService<T>();
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientProvider<T>(this IServiceCollection services) => services.AddTransient<Provider<T>>();
    }
}
