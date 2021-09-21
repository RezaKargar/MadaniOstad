using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MadaniOstad.IocConfig.CustomMapping
{
    public static class AutoMapperConfiguration
    {
        public static void InitializeAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new ConventionalMappingProfile());
            });
        }
    }
}
