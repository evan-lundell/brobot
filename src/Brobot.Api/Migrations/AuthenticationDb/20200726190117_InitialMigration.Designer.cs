﻿// <auto-generated />
using System;
using Brobot.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Brobot.Api.Migrations.AuthenticationDb
{
    [DbContext(typeof(AuthenticationDbContext))]
    [Migration("20200726190117_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiKey", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_date_utc")
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now() at time zone 'utc'");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnName("key")
                        .HasColumnType("character varying(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnName("owner")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("ApiKeyId");

                    b.ToTable("api_key","auth");
                });

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiKeyRole", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .HasColumnName("api_key_id")
                        .HasColumnType("integer");

                    b.Property<int>("ApiRoleId")
                        .HasColumnName("api_role_id")
                        .HasColumnType("integer");

                    b.HasKey("ApiKeyId", "ApiRoleId");

                    b.HasIndex("ApiRoleId");

                    b.ToTable("api_key_role","auth");
                });

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiRole", b =>
                {
                    b.Property<int>("ApiRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(20)")
                        .HasMaxLength(20);

                    b.HasKey("ApiRoleId");

                    b.ToTable("api_role","auth");
                });

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiKeyRole", b =>
                {
                    b.HasOne("Brobot.Api.Authentication.Entities.ApiKey", "ApiKey")
                        .WithMany("ApiKeyRoles")
                        .HasForeignKey("ApiKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Brobot.Api.Authentication.Entities.ApiRole", "ApiRole")
                        .WithMany("ApiKeyRoles")
                        .HasForeignKey("ApiRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}