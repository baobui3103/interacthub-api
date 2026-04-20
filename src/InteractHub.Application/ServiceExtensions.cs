using Microsoft.Extensions.DependencyInjection;
using InteractHub.Application.Interfaces;
using InteractHub.Application.Services;

namespace InteractHub.Application
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
        }
    }
}
