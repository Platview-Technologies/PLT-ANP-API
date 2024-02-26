using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Repository.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "Staff",
                    NormalizedName = "STAFF"
                });
               
        }

        //public static void SeedRoles(ModelBuilder modelBuilder, RoleManager<IdentityRole> roleManager)
        //{
        //    // Add roles if they don't exist
        //    if (!roleManager.RoleExistsAsync("Admin").Result)
        //    {
        //        var adminRole = new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" };
        //        roleManager.CreateAsync(adminRole).Wait();
        //    }

        //    if (!roleManager.RoleExistsAsync("Staff").Result)
        //    {
        //        var staffRole = new IdentityRole { Name = "Staff", NormalizedName = "STAFF" };
        //        roleManager.CreateAsync(staffRole).Wait();
        //    }
        //}
    }
}
