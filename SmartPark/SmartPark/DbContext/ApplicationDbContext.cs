using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartPark.Models;

namespace SmartPark.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Users> Users {  get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // seeding for  roles 

            var AdminId = Guid.NewGuid().ToString();
            var CustomerId = Guid.NewGuid().ToString();

            var roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Id = AdminId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                    ConcurrencyStamp = AdminId
                },
                
                new IdentityRole
                {
                    Id = CustomerId,
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper(),
                    ConcurrencyStamp = CustomerId
                } 
            };

            builder.Entity<IdentityRole>().HasData(roles);

            //seeding for admin

            var adminUser = new Users
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Aamir",
                Phone = "03457689432",
                Role = "Admin",
                CreationDate = DateTime.Now,
                UserName = "Aamir@gmail.com",
                NormalizedUserName = "Aamir@gmail.com",
                Email = "Aamir@gmail.com",
                NormalizedEmail = "Aamir@gmail.com",
                EmailConfirmed = true,
            };

            string adminPassword = "123";

            var passwordHasher = new PasswordHasher<Users>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

            builder.Entity<Users>().HasData(adminUser);

            // assign admin role to admin user

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = AdminId,
                UserId = adminUser.Id.ToString(),
            });

            builder.Entity<IdentityUserRole<string>>().HasKey(ar => new { ar.UserId, ar.RoleId });
            
            
        }

    }
}
