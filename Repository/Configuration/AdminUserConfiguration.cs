using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Configuration
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public static void SeedUsers(ModelBuilder modelBuilder, RoleManager<IdentityRole> roleManager, UserManager<UserModel> userManager)
        {
            // Instantiate admin user here
            //modelBuilder.Entity<UserModel>().HasData(
            //    new UserModel
            //    {
            //        Id = "1", // You may need to adjust the Id depending on your setup
            //        Email = "faseunOluwatobiloba@yahoo.com",
            //        UserName = "admin",
            //        PasswordHash = HashPassword("root123"),
            //        FirstName = "admin",
            //        EmailConfirmed = true,
            //        // Set other properties as needed
            //    });
            // Find the role IDs by name

            var adminRole = roleManager.FindByNameAsync("Admin").Result;
            var user = userManager.FindByEmailAsync("faseunOluwatobiloba@yahoo.com").Result;
            if (adminRole != null)
            {
                modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                    new IdentityUserRole<string>
                    {
                        UserId = user.Id, // Assuming the admin user has an Id of 1
                        RoleId = adminRole.Id
                    });
            }
        }
        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            // Hash the password using PasswordHasher<TUser>
            var hashedPassword = passwordHasher.HashPassword(null, password);
            return hashedPassword;
        }

        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            Guid guid = Guid.NewGuid();
            builder.HasData
                (
                new UserModel
                {
                    Id = "c9a646d3-9c61-4cb7-bfcd-ee2522c8f633",// You may need to adjust the Id depending on your setup
                    Email = "faseunOluwatobiloba@yahoo.com",
                    UserName = "admin",
                    PasswordHash = HashPassword("root123"),
                    FirstName = "admin",
                    EmailConfirmed = true,
                    // Set other properties as needed
                });
        }
    }
}
