using JobJournal.Data.Enums;
using JobJournal.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JobJournal.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();

                if(!context.Users.Any())
                {
                    var user = new User
                    {
                        fullName = "Cristiano Ronaldo",
                        email = "cristiano@gmail.com",
                        password = "cristiano@123",
                        passwordHash = "hashcristiano@123",
                        createdAt = DateTime.Now,
                    };
                    context.Users.Add(user);
                    context.SaveChanges();
                }


                if(!context.JobInfos.Any()) 
                {
                    var userId = context.Users.First().id;

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
                            userId = userId,
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
                            userId = userId,
                        }
                        ) ; 
                    context.SaveChanges() ;
                }
            } 
        }
    }
}
