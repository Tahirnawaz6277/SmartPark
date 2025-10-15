using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartPark.Services.Implementations;
using SmartPark.Services.Interfaces;
using System.Text;

namespace SmartPark.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.UseDIConfig();
            return services;
        }

        public static void UseDIConfig(this IServiceCollection services)
        {
            UserServices(services);
        }

        public static void UserServices(IServiceCollection services)
        {
            // register servicse here 
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHelper, Helper>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBillingService, BillingService>();



        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // jwt setup

            var key = configuration.GetValue<string>("Jwt:Key");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;

           
           
        }
    }
}
