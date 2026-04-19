using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InteractHub.Application.Core.Services;
using InteractHub.Domain.Core.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Infrastructure.Data;
using InteractHub.Infrastructure.Repositories;
using InteractHub.Infrastructure.Services;

namespace InteractHub.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<InteractHubDbContext>(options =>
                options.UseNpgsql("name=ConnectionStrings:InteractHubDatabase",
                x => x.MigrationsAssembly("InteractHub.Infrastructure")));

            services.AddAuthorization();

            services.AddIdentityApiEndpoints<ApplicationUser>()
                .AddEntityFrameworkStores<InteractHubDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddScoped(typeof(IBaseRepositoryAsync<>), typeof(BaseRepositoryAsync<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoggerService, LoggerService>();
        }

        public static void MigrateDatabase(this IServiceProvider serviceProvider)
        {
            // Sử dụng Scope để tránh lỗi memory leak khi resolve DbContext
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<InteractHubDbContext>();
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                }
            }
        }
    }
}