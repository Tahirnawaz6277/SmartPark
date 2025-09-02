using SmartPark.Data.Repositories.Implementations;
using SmartPark.Data.Repositories.Interfaces;

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
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


        }
    }
}
