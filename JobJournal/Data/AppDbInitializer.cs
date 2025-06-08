using JobJournal.Data.Enums;
using JobJournal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JobJournal.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                // UserManager and RoleManager services
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Seeding Users
                string adminUserEmail = "admin@example.com";
                if (await userManager.FindByEmailAsync(adminUserEmail) == null)
                {
                    var newAdminUser = new IdentityUser()
                    {
                        UserName = adminUserEmail,
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Password123!"); 
                    await userManager.AddToRoleAsync(newAdminUser, "Admin"); 
                }

                string normalUserEmail = "user@example.com";
                if (await userManager.FindByEmailAsync(normalUserEmail) == null)
                {
                    var newNormalUser = new IdentityUser()
                    {
                        UserName = normalUserEmail,
                        Email = normalUserEmail,
                        EmailConfirmed = true 
                    };
                    await userManager.CreateAsync(newNormalUser, "Password123!"); 
                    await userManager.AddToRoleAsync(newNormalUser, "User"); 
                }

                var seededUser = await userManager.FindByEmailAsync(adminUserEmail);
                string userIdToAssign = seededUser.Id;


                // Seeding JobInfos
                if (!context.JobInfos.Any())
                {
                    context.JobInfos.AddRange(
                        new JobInfo
                        {
                            companyName = "Apple",
                            role = "Software Engineer",
                            jobSummary = "AI tools and design work",
                            applicationStatus = ApplicationStatus.Applied,
                            appliedVia = AppliedVia.Email,
                            appliedTime = DateTime.Now,
                            notes = "super hopeful!",
                            userId = userIdToAssign, 
                        },

                        new JobInfo
                        {
                            companyName = "F1 Soft",
                            role = "Backend API developer",
                            jobSummary = "Scalable API design and implement",
                            applicationStatus = ApplicationStatus.InterviewScheduled,
                            appliedVia = AppliedVia.CompanyWebsite,
                            appliedTime = DateTime.Now.AddDays(-5),
                            notes = "preparing the best",
                            userId = userIdToAssign, 
                        }
                    );
                    await context.SaveChangesAsync(); 
                }
            } 
        }
    }
}
