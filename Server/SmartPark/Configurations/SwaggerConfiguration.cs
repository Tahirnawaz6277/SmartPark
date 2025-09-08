using Microsoft.OpenApi.Models;

namespace SmartPark.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwaggerAuthentication(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                //    option.SupportNonNullableReferenceTypes();
                //    option.OperationFilter<SwaggerFileOperationFilter>(); // Add support for files
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartPark", Version = "v1" });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Please enter the Bearer Authorization : `Bearer Genreated-JWT-Token`",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                 });

            });
        }
    }
}
