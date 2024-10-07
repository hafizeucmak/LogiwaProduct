using Logiwa.Common.Configurations;
using Logiwa.Common.Utils;
using Logiwa.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string AppSettings = "AppSettings";

        public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings));
            services.Configure<ConfigurationOptions>(appSettings);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));
        }

        public static void AddDbContext<TDbContext>(this IServiceCollection services, ConfigurationOptions configurationOptions)
        where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(options =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(StringBuilderUtils.BuildConnectionString(configurationOptions), sqlOptions =>
                {
                    sqlOptions.CommandTimeout(120);
                });
            });
        }

    }
}
