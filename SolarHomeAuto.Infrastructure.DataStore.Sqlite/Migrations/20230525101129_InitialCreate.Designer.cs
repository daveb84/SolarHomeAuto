﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolarHomeAuto.Infrastructure.DataStore;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    [Migration("20230525101129_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.AuthTokenEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccessToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId")
                        .IsUnique();

                    b.ToTable("AuthToken", (string)null);
                });

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.DeviceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("StateType")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DeviceId" }, "IX_Device_DeviceId");

                    b.ToTable("Devices", (string)null);
                });

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.DeviceHistoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Error")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DeviceId" }, "IX_DeviceHistory_DeviceId");

                    b.ToTable("DeviceHistory", (string)null);
                });

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.LogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Exception")
                        .HasColumnType("TEXT");

                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Logger")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Log", (string)null);
                });

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.UsageRealTimeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("BatteryCapacity")
                        .HasColumnType("TEXT");

                    b.Property<bool>("BatteryCharging")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("BatteryPower")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Consumption")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("GridPower")
                        .HasColumnType("TEXT");

                    b.Property<bool>("GridPurchasing")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("Production")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Date" }, "IX_UsageRealTime_Date");

                    b.ToTable("UsageRealTime", (string)null);
                });

            modelBuilder.Entity("SolarHomeAuto.Infrastructure.DataStore.Entities.UsageStatsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("ChargeCapacity")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Consumption")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("DischargeCapacity")
                        .HasColumnType("TEXT");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Generation")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("GridFeedIn")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("GridPurchase")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UsageStats", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
