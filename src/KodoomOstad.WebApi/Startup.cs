using KodoomOstad.DataAccessLayer;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.DataAccessLayer.Repositories;
using KodoomOstad.IocConfig.CustomMapping;
using KodoomOstad.IocConfig.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KodoomOstad.WebApi
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
            services.AddDbContext<KodoomOstadDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("KodoomOstadDatabase"));
            });

            services.InitializeAutoMapper();

            services.AddControllers();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}