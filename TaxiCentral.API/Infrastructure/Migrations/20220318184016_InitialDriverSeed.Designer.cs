﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaxiCentral.API.Infrastructure;

#nullable disable

namespace TaxiCentral.API.Infrastructure.Migrations
{
    [DbContext(typeof(TaxiCentralContext))]
    [Migration("20220318184016_InitialDriverSeed")]
    partial class InitialDriverSeed
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TaxiCentral.API.Models.Driver", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Pin")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Drivers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("3886af86-a48a-402e-87ea-1c5412d09f93"),
                            Name = "Darko",
                            Pin = "1234",
                            Surname = "Meshkovski"
                        });
                });

            modelBuilder.Entity("TaxiCentral.API.Models.Ride", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<double?>("Cost")
                        .HasColumnType("float");

                    b.Property<DateTime?>("DestinationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DriverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("EstimatedTimeOfArrival")
                        .HasColumnType("int");

                    b.Property<double?>("Mileage")
                        .HasColumnType("float");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TimeOfArrival")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.ToTable("Rides", (string)null);
                });

            modelBuilder.Entity("TaxiCentral.API.Models.Ride", b =>
                {
                    b.HasOne("TaxiCentral.API.Models.Driver", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("TaxiCentral.API.Models.LatLng", "ActualDestinationPoint", b1 =>
                        {
                            b1.Property<Guid>("RideId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float")
                                .HasColumnName("ActualDestinationLatitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float")
                                .HasColumnName("ActualDestinationLongitude");

                            b1.HasKey("RideId");

                            b1.ToTable("Rides");

                            b1.WithOwner()
                                .HasForeignKey("RideId");
                        });

                    b.OwnsOne("TaxiCentral.API.Models.LatLng", "ActualStartingPoint", b1 =>
                        {
                            b1.Property<Guid>("RideId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float")
                                .HasColumnName("ActualStartingLatitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float")
                                .HasColumnName("ActualStartingLongitude");

                            b1.HasKey("RideId");

                            b1.ToTable("Rides");

                            b1.WithOwner()
                                .HasForeignKey("RideId");
                        });

                    b.OwnsOne("TaxiCentral.API.Models.LatLng", "TargetDestinationPoint", b1 =>
                        {
                            b1.Property<Guid>("RideId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float")
                                .HasColumnName("TargetDestinationLatitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float")
                                .HasColumnName("TargetDestinationLongitude");

                            b1.HasKey("RideId");

                            b1.ToTable("Rides");

                            b1.WithOwner()
                                .HasForeignKey("RideId");
                        });

                    b.OwnsOne("TaxiCentral.API.Models.LatLng", "TargetStartingPoint", b1 =>
                        {
                            b1.Property<Guid>("RideId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float")
                                .HasColumnName("TargetStartingLatitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float")
                                .HasColumnName("TargetStartingLongitude");

                            b1.HasKey("RideId");

                            b1.ToTable("Rides");

                            b1.WithOwner()
                                .HasForeignKey("RideId");
                        });

                    b.Navigation("ActualDestinationPoint");

                    b.Navigation("ActualStartingPoint")
                        .IsRequired();

                    b.Navigation("Driver");

                    b.Navigation("TargetDestinationPoint");

                    b.Navigation("TargetStartingPoint");
                });
#pragma warning restore 612, 618
        }
    }
}
