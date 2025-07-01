using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartPark.DbContext
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()) // Make sure it matches the project directory
                            .AddJsonFile("appsettings.json")
                            .AddJsonFile("appsettings.Development.json", optional: true)
                            .Build();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SmartParkConnectionString"));

            return new ApplicationDbContext(optionsBuilder.Options);

        }
    }
}
