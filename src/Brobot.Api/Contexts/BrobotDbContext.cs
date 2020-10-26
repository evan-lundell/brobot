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
        public DbSet<EventResponse> EventResponses { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<SecretSantaGroup> SecretSantaGroups { get; set; }

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
            builder.Entity<DiscordUser>()
                .Property(du => du.BrobotAdmin)
                .HasColumnName("brobot_admin")
                .IsRequired(true)
                .HasDefaultValue(true);

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
                .HasForeignKey(er => er.DiscordEventId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<EventResponse>()
                .HasOne(er => er.Channel)
                .WithMany(c => c.EventResponses)
                .HasForeignKey(er => er.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

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

            builder.Entity<JobDefinition>()
                .ToTable(name: "job_definition", schema: "brobot")
                .HasKey(jd => jd.JobDefinitionId);
            builder.Entity<JobDefinition>()
                .Property(jd => jd.JobDefinitionId)
                .HasColumnName("id");
            builder.Entity<JobDefinition>()
                .Property(jd => jd.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(32);
            builder.Entity<JobDefinition>()
                .Property(jd => jd.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(1024);

            builder.Entity<JobParameterDefinition>()
                .ToTable(name: "job_parameter_definition", schema: "brobot")
                .HasKey(jpd => jpd.JobParameterDefinitionId);
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.JobParameterDefinitionId)
                .HasColumnName("id");
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.Name)
                .HasColumnName("name")
                .HasMaxLength(32)
                .IsRequired(true);
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(1024);
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.IsRequired)
                .HasColumnName("is_required");
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.UserConfigurable)
                .HasColumnName("user_configurable");
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.DataType)
                .IsRequired(true)
                .HasMaxLength(16)
                .HasColumnName("data_type");
            builder.Entity<JobParameterDefinition>()
                .Property(jpd => jpd.JobDefinitionId)
                .HasColumnName("job_definition_id");
            builder.Entity<JobParameterDefinition>()
                .HasOne(jpd => jpd.JobDefinition)
                .WithMany(jd => jd.JobParameterDefinitions)
                .HasForeignKey(jpd => jpd.JobDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Job>()
                .ToTable(name: "job", schema: "brobot")
                .HasKey(j => j.JobId);
            builder.Entity<Job>()
                .Property(j => j.JobId)
                .HasColumnName("id");
            builder.Entity<Job>()
                .Property(j => j.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(64);
            builder.Entity<Job>()
                .Property(j => j.Description)
                .HasColumnName("description")
                .HasMaxLength(1024)
                .IsRequired(false);
            builder.Entity<Job>()
                .Property(j => j.JobDefinitionId)
                .HasColumnName("job_definition_id");
            builder.Entity<Job>()
                .Property(j => j.CronTrigger)
                .HasColumnName("cron_trigger")
                .IsRequired(true)
                .HasMaxLength(16);
            builder.Entity<Job>()
                .Property(j => j.CreatedDateUtc)
                .HasColumnName("created_date_utc")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Entity<Job>()
                .Property(j => j.ModifiedDateUtc)
                .HasColumnName("modified_date_utc");
            builder.Entity<Job>()
                .HasOne(j => j.JobDefinition)
                .WithMany(jd => jd.Jobs)
                .HasForeignKey(j => j.JobDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobParameter>()
                .ToTable(name: "job_parameter", schema: "brobot")
                .HasKey(jp => jp.JobParameterId);
            builder.Entity<JobParameter>()
                .Property(jp => jp.JobParameterId)
                .HasColumnName("id");
            builder.Entity<JobParameter>()
                .Property(jp => jp.Value)
                .HasColumnName("value")
                .IsRequired(true)
                .HasMaxLength(1024);
            builder.Entity<JobParameter>()
                .Property(jp => jp.JobId)
                .HasColumnName("job_id");
            builder.Entity<JobParameter>()
                .Property(jp => jp.JobParameterDefinitionId)
                .HasColumnName("job_parameter_definition_id");
            builder.Entity<JobParameter>()
                .HasOne(jp => jp.Job)
                .WithMany(j => j.JobParameters)
                .HasForeignKey(jp => jp.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<JobParameter>()
                .HasOne(jp => jp.JobParameterDefinition)
                .WithMany(jpd => jpd.JobParameters)
                .HasForeignKey(jp => jp.JobParameterDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobChannel>()
                .ToTable(name: "job_channel", schema: "brobot")
                .HasKey(jc => new { jc.ChannelId, jc.JobId });
            builder.Entity<JobChannel>()
                .Property(jc => jc.JobId)
                .HasColumnName("job_id");
            builder.Entity<JobChannel>()
                .Property(jc => jc.ChannelId)
                .HasColumnName("channel_id");
            builder.Entity<JobChannel>()
                .HasOne(jc => jc.Job)
                .WithMany(j => j.JobChannels)
                .HasForeignKey(jc => jc.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<JobChannel>()
                .HasOne(jc => jc.Channel)
                .WithMany(c => c.JobChannels)
                .HasForeignKey(jc => jc.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SecretSantaGroup>()
                .ToTable(name: "secret_santa_group", schema: "brobot")
                .HasKey(ssg => ssg.SecretSantaGroupId);
            builder.Entity<SecretSantaGroup>()
                .Property(ssg => ssg.SecretSantaGroupId)
                .HasColumnName("id");
            builder.Entity<SecretSantaGroup>()
                .Property(ssg => ssg.Name)
                .HasColumnName("name")
                .IsRequired(true)
                .HasMaxLength(100);
            builder.Entity<SecretSantaGroup>()
                .Property(ssg => ssg.CheckPastYearPairings)
                .HasColumnName("check_past_year_pairings")
                .HasDefaultValue(true)
                .IsRequired(true);

            builder.Entity<SecretSantaGroupDiscordUser>()
                .ToTable(name: "secret_santa_group_discord_user", schema: "brobot")
                .HasKey(ssgdu => new { ssgdu.SecretSantaGroupId, ssgdu.DiscordUserId });
            builder.Entity<SecretSantaGroupDiscordUser>()
                .Property(ssgdu => ssgdu.DiscordUserId)
                .HasColumnName("discord_user_id");
            builder.Entity<SecretSantaGroupDiscordUser>()
                .Property(ssgdu => ssgdu.SecretSantaGroupId)
                .HasColumnName("secret_santa_group_id");
            builder.Entity<SecretSantaGroupDiscordUser>()
                .HasOne(ssgdu => ssgdu.SecretSantaGroup)
                .WithMany(ssg => ssg.SecretSantaGroupDiscordUsers)
                .HasForeignKey(ssgdu => ssgdu.SecretSantaGroupId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SecretSantaGroupDiscordUser>()
                .HasOne(ssgdu => ssgdu.DiscordUser)
                .WithMany(du => du.SecretSantaGroupDiscordUsers)
                .HasForeignKey(ssgdu => ssgdu.DiscordUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SecretSantaEvent>()
                .ToTable("secret_santa_event")
                .HasKey(sse => sse.SecretSantaEventId);
            builder.Entity<SecretSantaEvent>()
                .Property(sse => sse.SecretSantaEventId)
                .HasColumnName("id");
            builder.Entity<SecretSantaEvent>()
                .Property(sse => sse.Year)
                .HasColumnName("year");
            builder.Entity<SecretSantaEvent>()
                .Property(sse => sse.CreatedDateUtc)
                .HasColumnName("created_date_utc")
                .HasDefaultValueSql("now() at time zone 'utc'");
            builder.Entity<SecretSantaEvent>()
                .Property(sse => sse.CreatedById)
                .HasColumnName("created_by_id");
            builder.Entity<SecretSantaEvent>()
                .Property(sse => sse.SecretSantaGroupId)
                .HasColumnName("secret_santa_group_id");
            builder.Entity<SecretSantaEvent>()
                .HasOne(sse => sse.CreatedBy)
                .WithMany(du => du.SecretSantaEvents)
                .HasForeignKey(sse => sse.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<SecretSantaEvent>()
                .HasOne(sse => sse.SecretSantaGroup)
                .WithMany(ssg => ssg.SecretSantaEvents)
                .HasForeignKey(sse => sse.SecretSantaGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SecretSantaPairing>()
                .ToTable("secret_santa_pairing")
                .HasKey(ssp => ssp.SecretSantaPairingId);
            builder.Entity<SecretSantaPairing>()
                .Property(ssp => ssp.SecretSantaPairingId)
                .HasColumnName("id");
            builder.Entity<SecretSantaPairing>()
                .Property(ssp => ssp.SecretSantaEventId)
                .HasColumnName("secret_santa_event_id");
            builder.Entity<SecretSantaPairing>()
                .Property(ssp => ssp.GiverId)
                .HasColumnName("giver_id");
            builder.Entity<SecretSantaPairing>()
                .Property(ssp => ssp.RecipientId)
                .HasColumnName("recipient_id");
            builder.Entity<SecretSantaPairing>()
                .HasOne(ssp => ssp.SecretSantaEvent)
                .WithMany(sse => sse.SecretSantaPairings)
                .HasForeignKey(ssp => ssp.SecretSantaEventId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SecretSantaPairing>()
                .HasOne(ssp => ssp.Recipient)
                .WithMany(du => du.RecipientPairings)
                .HasForeignKey(ssp => ssp.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<SecretSantaPairing>()
                .HasOne(ssp => ssp.Giver)
                .WithMany(du => du.GiverPairings)
                .HasForeignKey(ssp => ssp.GiverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
