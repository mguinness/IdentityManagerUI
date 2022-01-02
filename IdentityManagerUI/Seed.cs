using IdentityManagerUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagerUI
{
    public static class Seed
    {
        public static void AddAdmin(this ModelBuilder builder, string email = "admin@admin.com", string password = "Admin*123")
        {
            SeedUser(builder, email, password);
            SeedRole(builder);
            SeedUserRole(builder);
        }
        private static void SeedUser(ModelBuilder builder, string email, string password)
        {
            ApplicationUser admin = new ApplicationUser();
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            admin.Id = "b74ddd14-6340-4840-95c2-db12554843e5";
            admin.UserName = email;
            admin.Email = email;
            admin.LockoutEnabled = false;
            admin.NormalizedEmail = email.ToUpper();
            admin.NormalizedUserName = email.ToUpper();
            admin.EmailConfirmed = true;
            admin.PasswordHash = passwordHasher.HashPassword(admin, password);
            builder.Entity<ApplicationUser>().HasData(admin);
        }
        private static void SeedRole(ModelBuilder builder)
        {
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole()
                {
                    Id = "fab4fac1-c546-41de-aebc-a14da6895711",
                    Name = "Admin",
                    ConcurrencyStamp = "1",
                    NormalizedName = "Admin"
                });
        }
        private static void SeedUserRole(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = "fab4fac1-c546-41de-aebc-a14da6895711",
                    UserId = "b74ddd14-6340-4840-95c2-db12554843e5"
                });
        }

    }
}
