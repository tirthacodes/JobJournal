using JobJournal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Export.HtmlExport.StyleCollectors.StyleContracts;

namespace JobJournal.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<JobInfo> JobInfos { get; set; }
        public DbSet<JobImage> JobImages { get; set; }
        public DbSet<CvProfile> CvProfiles { get; set; } 
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // calling base.OnModelCreating for Identity
        }

    }
}
