using JobJournal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobJournal.Data
{
    public class AppDbContnext : DbContext
    {
        public AppDbContnext(DbContextOptions<AppDbContnext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<JobInfo> JobInfos { get; set; }

    }
}
