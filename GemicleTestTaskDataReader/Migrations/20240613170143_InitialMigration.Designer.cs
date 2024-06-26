﻿// <auto-generated />
using System;
using GemicleTestTaskData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GemicleTestTaskData.Migrations
{
    [DbContext(typeof(CampaignDbContext))]
    [Migration("20240613170143_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.Campaign", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime>("ScheduledTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Deposit")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NewCustomer")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.CustomerCampaignSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AlreadySent")
                        .HasColumnType("bit");

                    b.Property<Guid>("CampaignId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ToSendDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerCampaignSchedule");
                });

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.CustomerCampaignSchedule", b =>
                {
                    b.HasOne("GemicleTestTaskModels.DBModels.Campaign", "Campaign")
                        .WithMany("CustomerLogs")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GemicleTestTaskModels.DBModels.Customer", "Customer")
                        .WithMany("CustomerLogs")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.Campaign", b =>
                {
                    b.Navigation("CustomerLogs");
                });

            modelBuilder.Entity("GemicleTestTaskModels.DBModels.Customer", b =>
                {
                    b.Navigation("CustomerLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
