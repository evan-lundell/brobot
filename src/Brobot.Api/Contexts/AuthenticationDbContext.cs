using Brobot.Api.Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Contexts
{
    public class AuthenticationDbContext : DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApiKey> ApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApiKey>()
                .ToTable("ApiKey", schema: "auth")
                .HasKey(ak => ak.ApiKeyId);
            builder.Entity<ApiKey>()
                .Property(ak => ak.Key)
                .HasMaxLength(36)
                .IsRequired(true);
            builder.Entity<ApiKey>()
                .Property(ak => ak.CreatedDateUtc)
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Entity<ApiKey>()
                .Property(ak => ak.Owner)
                .HasMaxLength(50)
                .IsRequired(true);

            builder.Entity<ApiRole>()
                .ToTable("ApiRole", schema: "auth")
                .HasKey(ar => ar.ApiRoleId);
            builder.Entity<ApiRole>()
                .Property(ar => ar.Name)
                .IsRequired(true)
                .HasMaxLength(20);

            builder.Entity<ApiKeyRole>()
                .ToTable("ApiKeyRole", schema: "auth")
                .HasKey(akr => new { akr.ApiKeyId, akr.ApiRoleId });
            builder.Entity<ApiKeyRole>()
                .HasOne(akr => akr.ApiKey)
                .WithMany(ak => ak.ApiKeyRoles)
                .HasForeignKey(akr => akr.ApiKeyId);
            builder.Entity<ApiKeyRole>()
                .HasOne(akr => akr.ApiRole)
                .WithMany(ar => ar.ApiKeyRoles)
                .HasForeignKey(akr => akr.ApiRoleId);
        }
    }
}
