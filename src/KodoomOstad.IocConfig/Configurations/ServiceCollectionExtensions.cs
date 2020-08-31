using KodoomOstad.Common;
using KodoomOstad.Common.Exceptions;
using KodoomOstad.Common.Utilities;
using KodoomOstad.DataAccessLayer;
using KodoomOstad.Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception != null)
                            throw new UnauthorizedException(context.Exception.Message);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            context.Fail("This token has no security stamp");

                        var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                        if (validatedUser == null)
                            context.Fail("Token security stamp is not valid.");
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                            throw new UnauthorizedException(context.Error);


                        throw new UnauthorizedException("Authentication needed");
                    }
                };
            });
        }
    }
}
