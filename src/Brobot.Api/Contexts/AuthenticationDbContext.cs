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
                .ToTable("api_key", schema: "auth")
                .HasKey(ak => ak.ApiKeyId);
            builder.Entity<ApiKey>()
                .Property(ak => ak.ApiKeyId)
                .HasColumnName("id");
            builder.Entity<ApiKey>()
                .Property(ak => ak.Key)
                .HasColumnName("key")
                .HasMaxLength(36)
                .IsRequired(true);
            builder.Entity<ApiKey>()
                .Property(ak => ak.CreatedDateUtc)
                .HasColumnName("created_date_utc")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Entity<ApiKey>()
                .Property(ak => ak.Owner)
                .HasColumnName("owner")
                .HasMaxLength(50)
                .IsRequired(true);

            builder.Entity<ApiRole>()
                .ToTable("api_role", schema: "auth")
                .HasKey(ar => ar.ApiRoleId);
            builder.Entity<ApiRole>()
                .Property(ar => ar.ApiRoleId)
                .HasColumnName("id");
            builder.Entity<ApiRole>()
                .Property(ar => ar.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(20);

            builder.Entity<ApiKeyRole>()
                .ToTable("api_key_role", schema: "auth")
                .HasKey(akr => new { akr.ApiKeyId, akr.ApiRoleId });
            builder.Entity<ApiKeyRole>()
                .Property(akr => akr.ApiKeyId)
                .HasColumnName("api_key_id");
            builder.Entity<ApiKeyRole>()
                .Property(akr => akr.ApiRoleId)
                .HasColumnName("api_role_id");
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
