﻿using Brobot.Api.Entities;
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
        public DbSet<EventResponse> EventResponses { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Server>()
                .ToTable(name: "server", schema: "brobot")
                .HasKey(s => s.ServerId);
            builder.Entity<Server>()
                .Property(s => s.ServerId)
                .HasColumnName("id");
            builder.Entity<Server>()
                .Property(s => s.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(128);

            builder.Entity<Channel>()
                .ToTable(name: "channel", schema: "brobot")
                .HasKey(c => c.ChannelId);
            builder.Entity<Channel>()
                .Property(c => c.ChannelId)
                .HasColumnName("id");
            builder.Entity<Channel>()
                .Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(128);
            builder.Entity<Channel>()
                .Property(c => c.ServerId)
                .HasColumnName("server_id");
            builder.Entity<Channel>()
                .HasOne(c => c.Server)
                .WithMany(s => s.Channels)
                .HasForeignKey(c => c.ServerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DiscordUser>()
                .ToTable(name: "discord_user", schema: "brobot")
                .HasKey(du => du.DiscordUserId);
            builder.Entity<DiscordUser>()
                .Property(du => du.DiscordUserId)
                .HasColumnName("id");
            builder.Entity<DiscordUser>()
                .Property(du => du.Username)
                .HasColumnName("username")
                .IsRequired(true)
                .HasMaxLength(128);
            builder.Entity<DiscordUser>()
                .Property(du => du.Birthdate)
                .HasColumnName("birthdate");
            builder.Entity<DiscordUser>()
                .Property(du => du.Timezone)
                .HasColumnName("timezone")
                .IsRequired(false)
                .HasMaxLength(64);

            builder.Entity<DiscordUserChannel>()
                .ToTable(name: "discord_user_channel", schema: "brobot")
                .HasKey(duc => new { duc.DiscordUserId, duc.ChannelId });
            builder.Entity<DiscordUserChannel>()
                .Property(duc => duc.ChannelId)
                .HasColumnName("channel_id");
            builder.Entity<DiscordUserChannel>()
                .Property(duc => duc.DiscordUserId)
                .HasColumnName("discord_user_id");
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
            
            builder.Entity<DiscordEvent>()
                .ToTable(name: "discord_event", schema: "brobot")
                .HasKey(de => de.DiscordEventId);
            builder.Entity<DiscordEvent>()
                .Property(de => de.DiscordEventId)
                .HasColumnName("id");
            builder.Entity<DiscordEvent>()
                .Property(de => de.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(64);

            builder.Entity<EventResponse>()
                .ToTable(name: "event_response", schema: "brobot")
                .HasKey(er => er.EventResponseId);
            builder.Entity<EventResponse>()
                .Property(er => er.EventResponseId)
                .HasColumnName("id");
            builder.Entity<EventResponse>()
                .Property(er => er.MessageText)
                .HasColumnName("message_text")
                .IsRequired(false)
                .HasMaxLength(1024);
            builder.Entity<EventResponse>()
                .Property(er => er.ResponseText)
                .HasColumnName("response_text")
                .IsRequired(true)
                .HasMaxLength(1024);
            builder.Entity<EventResponse>()
                .Property(er => er.ChannelId)
                .HasColumnName("channel_id");
            builder.Entity<EventResponse>()
                .Property(er => er.DiscordEventId)
                .HasColumnName("discord_event_id");
            builder.Entity<EventResponse>()
                .HasOne(er => er.DiscordEvent)
                .WithMany(de => de.EventResponses)
                .HasForeignKey(er => er.DiscordEventId);
            builder.Entity<EventResponse>()
                .HasOne(er => er.Channel)
                .WithMany(c => c.EventResponses)
                .HasForeignKey(er => er.ChannelId);

            builder.Entity<Reminder>()
                .ToTable(name: "reminder", schema: "brobot")
                .HasKey(r => r.ReminderId);
            builder.Entity<Reminder>()
                .Property(r => r.ReminderId)
                .HasColumnName("id");
            builder.Entity<Reminder>()
                .Property(r => r.OwnerId)
                .HasColumnName("owner_id");
            builder.Entity<Reminder>()
                .Property(r => r.ChannelId)
                .HasColumnName("channel_id");
            builder.Entity<Reminder>()
                .Property(r => r.Message)
                .HasColumnName("message")
                .IsRequired(true)
                .HasMaxLength(1024);
            builder.Entity<Reminder>()
                .Property(r => r.CreatedDateUtc)
                .HasColumnName("created_date_utc")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Entity<Reminder>()
                .Property(r => r.ReminderDateUtc)
                .HasColumnName("reminder_date_utc");
            builder.Entity<Reminder>()
                .Property(r => r.SentDateUtc)
                .HasColumnName("sent_date_utc");
        }
    }
}
