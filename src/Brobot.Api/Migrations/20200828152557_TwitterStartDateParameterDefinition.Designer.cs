﻿// <auto-generated />
using System;
using Brobot.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations
{
    [DbContext(typeof(BrobotDbContext))]
    [Migration("20200828152557_TwitterStartDateParameterDefinition")]
    partial class TwitterStartDateParameterDefinition
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Brobot.Api.Entities.Channel", b =>
                {
                    b.Property<decimal>("ChannelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<decimal>("ServerId")
                        .HasColumnName("server_id")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("ChannelId");

                    b.HasIndex("ServerId");

                    b.ToTable("channel","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.DiscordEvent", b =>
                {
                    b.Property<int>("DiscordEventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.HasKey("DiscordEventId");

                    b.ToTable("discord_event","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.DiscordUser", b =>
                {
                    b.Property<decimal>("DiscordUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnName("birthdate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Timezone")
                        .HasColumnName("timezone")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnName("username")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.HasKey("DiscordUserId");

                    b.ToTable("discord_user","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.DiscordUserChannel", b =>
                {
                    b.Property<decimal>("DiscordUserId")
                        .HasColumnName("discord_user_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("ChannelId")
                        .HasColumnName("channel_id")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("DiscordUserId", "ChannelId");

                    b.HasIndex("ChannelId");

                    b.ToTable("discord_user_channel","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.EventResponse", b =>
                {
                    b.Property<int>("EventResponseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal?>("ChannelId")
                        .HasColumnName("channel_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("DiscordEventId")
                        .HasColumnName("discord_event_id")
                        .HasColumnType("integer");

                    b.Property<string>("MessageText")
                        .HasColumnName("message_text")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.Property<string>("ResponseText")
                        .IsRequired()
                        .HasColumnName("response_text")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.HasKey("EventResponseId");

                    b.HasIndex("ChannelId");

                    b.HasIndex("DiscordEventId");

                    b.ToTable("event_response","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_date_utc")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("CronTrigger")
                        .IsRequired()
                        .HasColumnName("cron_trigger")
                        .HasColumnType("character varying(16)")
                        .HasMaxLength(16);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.Property<int>("JobDefinitionId")
                        .HasColumnName("job_definition_id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("ModifiedDateUtc")
                        .HasColumnName("modified_date_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.HasKey("JobId");

                    b.HasIndex("JobDefinitionId");

                    b.ToTable("job","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobChannel", b =>
                {
                    b.Property<decimal>("ChannelId")
                        .HasColumnName("channel_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("JobId")
                        .HasColumnName("job_id")
                        .HasColumnType("integer");

                    b.HasKey("ChannelId", "JobId");

                    b.HasIndex("JobId");

                    b.ToTable("job_channel","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobDefinition", b =>
                {
                    b.Property<int>("JobDefinitionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(32)")
                        .HasMaxLength(32);

                    b.HasKey("JobDefinitionId");

                    b.ToTable("job_definition","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobParameter", b =>
                {
                    b.Property<int>("JobParameterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("JobId")
                        .HasColumnName("job_id")
                        .HasColumnType("integer");

                    b.Property<int>("JobParameterDefinitionId")
                        .HasColumnName("job_parameter_definition_id")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnName("value")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.HasKey("JobParameterId");

                    b.HasIndex("JobId");

                    b.HasIndex("JobParameterDefinitionId");

                    b.ToTable("job_parameter","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobParameterDefinition", b =>
                {
                    b.Property<int>("JobParameterDefinitionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasColumnName("data_type")
                        .HasColumnType("character varying(16)")
                        .HasMaxLength(16);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.Property<bool>("IsRequired")
                        .HasColumnName("is_required")
                        .HasColumnType("boolean");

                    b.Property<int>("JobDefinitionId")
                        .HasColumnName("job_definition_id")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(32)")
                        .HasMaxLength(32);

                    b.Property<bool>("UserConfigurable")
                        .HasColumnName("user_configurable")
                        .HasColumnType("boolean");

                    b.HasKey("JobParameterDefinitionId");

                    b.HasIndex("JobDefinitionId");

                    b.ToTable("job_parameter_definition","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.Reminder", b =>
                {
                    b.Property<int>("ReminderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("ChannelId")
                        .HasColumnName("channel_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("CreatedDateUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_date_utc")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnName("message")
                        .HasColumnType("character varying(1024)")
                        .HasMaxLength(1024);

                    b.Property<decimal>("OwnerId")
                        .HasColumnName("owner_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("ReminderDateUtc")
                        .HasColumnName("reminder_date_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("SentDateUtc")
                        .HasColumnName("sent_date_utc")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("ReminderId");

                    b.HasIndex("ChannelId");

                    b.HasIndex("OwnerId");

                    b.ToTable("reminder","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.Server", b =>
                {
                    b.Property<decimal>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.HasKey("ServerId");

                    b.ToTable("server","brobot");
                });

            modelBuilder.Entity("Brobot.Api.Entities.Channel", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Server", "Server")
                        .WithMany("Channels")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.DiscordUserChannel", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Channel", "Channel")
                        .WithMany("DiscordUserChannels")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Brobot.Api.Entities.DiscordUser", "DiscordUser")
                        .WithMany("DiscordUserChannels")
                        .HasForeignKey("DiscordUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.EventResponse", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Channel", "Channel")
                        .WithMany("EventResponses")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Brobot.Api.Entities.DiscordEvent", "DiscordEvent")
                        .WithMany("EventResponses")
                        .HasForeignKey("DiscordEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.Job", b =>
                {
                    b.HasOne("Brobot.Api.Entities.JobDefinition", "JobDefinition")
                        .WithMany("Jobs")
                        .HasForeignKey("JobDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobChannel", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Channel", "Channel")
                        .WithMany("JobChannels")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Brobot.Api.Entities.Job", "Job")
                        .WithMany("JobChannels")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobParameter", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Job", "Job")
                        .WithMany("JobParameters")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Brobot.Api.Entities.JobParameterDefinition", "JobParameterDefinition")
                        .WithMany("JobParameters")
                        .HasForeignKey("JobParameterDefinitionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.JobParameterDefinition", b =>
                {
                    b.HasOne("Brobot.Api.Entities.JobDefinition", "JobDefinition")
                        .WithMany("JobParameterDefinitions")
                        .HasForeignKey("JobDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Brobot.Api.Entities.Reminder", b =>
                {
                    b.HasOne("Brobot.Api.Entities.Channel", "Channel")
                        .WithMany("Reminders")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Brobot.Api.Entities.DiscordUser", "Owner")
                        .WithMany("Reminders")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
