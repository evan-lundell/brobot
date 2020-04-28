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
        public DbSet<Channel> Channels { get; set; }
        public DbSet<DiscordUser> DiscordUsers { get; set; }
        public DbSet<DiscordUserChannel> DiscordUserChannels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Server>()
                .ToTable("Server")
                .HasKey(s => s.ServerId);
            builder.Entity<Server>()
                .Property(s => s.Name)
                .IsRequired(true)
                .HasMaxLength(32);

            builder.Entity<Channel>()
                .ToTable("Channel")
                .HasKey(c => c.ChannelId);
            builder.Entity<Channel>()
                .Property(c => c.Name)
                .IsRequired(true)
                .HasMaxLength(32);
            builder.Entity<Channel>()
                .HasOne(c => c.Server)
                .WithMany(s => s.Channels)
                .HasForeignKey(c => c.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DiscordUser>()
                .ToTable("DiscordUser")
                .HasKey(du => du.DiscordUserId);
            builder.Entity<DiscordUser>()
                .Property(du => du.Username)
                .IsRequired(true)
                .HasMaxLength(32);
            builder.Entity<DiscordUser>()
                .Property(du => du.Timezone)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Entity<DiscordUserChannel>()
                .ToTable("DiscordUserChannel")
                .HasKey(duc => new { duc.DiscordUserId, duc.ChannelId });
            builder.Entity<DiscordUserChannel>()
                .HasOne(duc => duc.DiscordUser)
                .WithMany(du => du.DiscordUserChannels)
                .HasForeignKey(duc => duc.DiscordUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DiscordUserChannel>()
                .HasOne(duc => duc.Channel)
                .WithMany(c => c.DiscordUserChannels)
                .HasForeignKey(duc => duc.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
