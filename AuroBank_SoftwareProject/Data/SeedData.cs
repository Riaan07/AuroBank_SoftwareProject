
using AuroBank_SoftwareProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuroBank_SoftwareProject.Data
{
    public static class SeedData
    {
        private static readonly string password = "Pass.W0rd";
        private static readonly List<string> roles = new()
        { "Admin", "Consultant", "Advisor", "User" };

        private static readonly List<AppUser> appUsers = new()
        {
            new AppUser
            {
                UserName = "dlaminir@ufs.ac.za",
                FirstName = "Riaan",
                LastName = "Dlamini",
                Email = "dlaminir@ufs.ac.za",
                StudentStaffNumber = "088567",
                AccountNumber = "1111111001",
            },
            new AppUser
            {
                UserName = "kingp@ufs.ac.za",
                FirstName = "Prince",
                LastName = "King",
                Email = "kingp@ufs.ac.za",
                StudentStaffNumber = "088568",
                AccountNumber = "1111111002",
            },
            new AppUser
            {
                UserName = "dlaminis@ufs.ac.za",
                FirstName = "Sbahle",
                LastName = "Dlamini",
                Email = "dlaminis@ufs.ac.za",
                StudentStaffNumber = "089569",
                AccountNumber = "1111111003",
            }
        };

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BankDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create users if they don't exist
            foreach (var appUser in appUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(appUser.Email);
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(appUser, password);
                    if (result.Succeeded)
                    {
                        // Assign role based on user type
                        if (appUser.UserName.Contains("consultant"))
                            await userManager.AddToRoleAsync(appUser, "Consultant");
                        else if (appUser.UserName.Contains("admin"))
                            await userManager.AddToRoleAsync(appUser, "Admin");
                        else if (appUser.UserName.Contains("advisor"))
                            await userManager.AddToRoleAsync(appUser, "Advisor");
                    }
                }
            }
        }
    }
}
