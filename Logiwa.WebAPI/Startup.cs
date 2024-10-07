using Logiwa.Common.Configurations;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.WebAPI.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace Logiwa.WebAPI
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        public readonly string _apiTitle = "Logiwa Product API";
        public Startup(ILogger<Startup> logger, IConfiguration configuration, IHostEnvironment env)
        {
            Logger = logger;
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public ILogger<Startup> Logger { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            var configurationOptions = Configuration.GetSection("AppSettings").Get<ConfigurationOptions>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = _apiTitle, Version = "v1" });
            });


            if (configurationOptions != null)
            {
                services.AddDbContext<BaseDbContext>(configurationOptions);
            }

            services.AddMediatR(m => m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        private void ConfigureSwaggerUI(SwaggerUIOptions options)
        {
            options.DocExpansion(DocExpansion.None);
            options.DisplayRequestDuration();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", _apiTitle);
            options.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
            options.InjectJavascript("../js/swagger-seed-dropdown-sorting.js");
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(ConfigureSwaggerUI);
            }

            app.UseMiddleware<Middlewares.ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}