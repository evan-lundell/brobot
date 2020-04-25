using Brobot.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Contexts
{
    public class BrobotDbContext : DbContext
    {
        public BrobotDbContext(DbContextOptions<BrobotDbContext> options)
            : base(options)
        {
        }

        public DbSet<Server> Servers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Server>()
                .ToTable("Server")
                .HasKey(s => s.ServerId);
            builder.Entity<Server>()
                .Property(s => s.Name)
                .IsRequired(true)
                .HasMaxLength(32);
        }

    }
}
