using MadaniOstad.Common;
using MadaniOstad.DataAccessLayer;
using MadaniOstad.DataAccessLayer.Contracts;
using MadaniOstad.DataAccessLayer.Repositories;
using MadaniOstad.IocConfig.Configurations;
using MadaniOstad.IocConfig.CustomMapping;
using MadaniOstad.IocConfig.Middlewares;
using MadaniOstad.IocConfig.SwaggerConfigurations;
using MadaniOstad.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace MadaniOstad.WebApi
{
    public class Startup
    {
        private readonly Settings _siteSetting;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _siteSetting = configuration.GetSection(nameof(Settings)).Get<Settings>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Settings>(Configuration.GetSection(nameof(Settings)));

            services.AddDbContext<MadaniOstadDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("MadaniOstadDatabase"));
            });

            services.AddCors();

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.InitializeAutoMapper();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IJwtService, JwtService>();

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandler();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseSwaggerAndUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
        }
    }
}