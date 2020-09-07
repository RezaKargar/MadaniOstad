using KodoomOstad.Common;
using KodoomOstad.DataAccessLayer;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.DataAccessLayer.Repositories;
using KodoomOstad.IocConfig.Configurations;
using KodoomOstad.IocConfig.CustomMapping;
using KodoomOstad.IocConfig.Middlewares;
using KodoomOstad.IocConfig.SwaggerConfigurations;
using KodoomOstad.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KodoomOstad.WebApi
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

            services.AddDbContext<KodoomOstadDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("KodoomOstadDatabase"));
            });

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            services.InitializeAutoMapper();

            services.AddControllers();

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