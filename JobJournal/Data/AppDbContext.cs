using JobJournal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobJournal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<JobInfo> JobInfos { get; set; }

    }
}
