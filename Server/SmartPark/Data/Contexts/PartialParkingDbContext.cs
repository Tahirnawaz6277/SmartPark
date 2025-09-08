using Microsoft.EntityFrameworkCore;
using SmartPark.Models;

namespace SmartPark.Data.Contexts
{
    public partial class ParkingDbContext  
    {
        // Here we extend the DbContext
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // global query filter for soft delete
            modelBuilder.Entity<User>()
                        .HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
