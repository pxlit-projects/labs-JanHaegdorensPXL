using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    using DevOps.Domain;
    using Microsoft.EntityFrameworkCore;

    public class DevOpsContext : DbContext
    {
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Team> Teams { get; set; }

        public DevOpsContext(DbContextOptions<DevOpsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new TeamConfiguration().Configure(modelBuilder.Entity<Team>());
            new DeveloperConfiguration().Configure(modelBuilder.Entity<Developer>());
        }
    }

}
