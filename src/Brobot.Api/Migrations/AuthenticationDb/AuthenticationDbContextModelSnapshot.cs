﻿// <auto-generated />
using System;
using Brobot.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brobot.Api.Migrations.AuthenticationDb
{
    [DbContext(typeof(AuthenticationDbContext))]
    partial class AuthenticationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiKey", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDateUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("ApiKeyId");

                    b.ToTable("ApiKey","auth");
                });

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiKeyRole", b =>
                {
                    b.Property<int>("ApiKeyId")
                        .HasColumnType("int");

                    b.Property<int>("ApiRoleId")
                        .HasColumnType("int");

                    b.HasKey("ApiKeyId", "ApiRoleId");

                    b.HasIndex("ApiRoleId");

                    b.ToTable("ApiKeyRole","auth");
                });

            modelBuilder.Entity("Brobot.Api.Authentication.Entities.ApiRole", b =>
                {
                    b.Property<int>("ApiRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("ApiRoleId");

                    b.ToTable("ApiRole","auth");
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
