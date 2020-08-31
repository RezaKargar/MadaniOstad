using KodoomOstad.Common;
using KodoomOstad.DataAccessLayer;
using KodoomOstad.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace KodoomOstad.IocConfig.Configurations
{
    public static class ServiceCollectionExtensions
    {

        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            services.AddIdentity<User, IdentityRole<int>>(identityOptions =>
                {
                    //Password Settings
                    identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
                    identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
                    identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric;
                    identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
                    identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

                    //UserName Settings
                    identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;
                })
                .AddEntityFrameworkStores<KodoomOstadDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
