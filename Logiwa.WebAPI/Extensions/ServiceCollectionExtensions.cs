using Logiwa.Business.DTOs.Categories.Outputs;
using Logiwa.Common.Configurations;
using Logiwa.Common.Exceptions;
using Logiwa.Common.Utils;
using Logiwa.Infrastructure.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

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
            services.AddScoped(typeof(IGenericReadRepository<>), typeof(GenericReadRepository<>));
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
        public static void AddExceptionManager(this IServiceCollection services)
        {
            var exceptionManagerConfig = GetExceptionManagerConfig();
            services.AddSingleton<IExceptionManager>(manager => new ExceptionManager(exceptionManagerConfig));
        }

        public static Dictionary<Type, int> GetExceptionManagerConfig()
        {
            Dictionary<Type, int> exceptionManagerConfig = new Dictionary<Type, int>();
            exceptionManagerConfig.Add(typeof(ResourceNotFoundException), (int)HttpStatusCode.NotFound);
            exceptionManagerConfig.Add(typeof(AlreadyExistsException), (int)HttpStatusCode.Created);

            return exceptionManagerConfig;
        }
        public static void AddMapster(this IServiceCollection services)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            Assembly applicationAssembly = typeof(CategoryOutputDTO).Assembly;
            typeAdapterConfig.Scan(applicationAssembly);
        }
    }
}
