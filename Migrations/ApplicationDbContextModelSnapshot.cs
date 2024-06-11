﻿// <auto-generated />
using System;
using ArdaProje.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ArdaProje.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ArdaProje.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentID"), 1L, 1);

                    b.Property<DateTime>("AppointmentDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PTID")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("AppointmentID");

                    b.HasIndex("PTID");

                    b.HasIndex("UserID");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("ArdaProje.Models.ClosedSlots", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("ClosedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PTID")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PTID");

                    b.ToTable("ClosedSlots");
                });

            modelBuilder.Entity("ArdaProje.Models.PT", b =>
                {
                    b.Property<int>("PTID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PTID"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExpertiseArea")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PTID");

                    b.ToTable("PTs");
                });

            modelBuilder.Entity("ArdaProje.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PTID")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RemainingLessons")
                        .HasColumnType("int");

                    b.HasKey("UserID");

                    b.HasIndex("PTID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ArdaProje.Models.Appointment", b =>
                {
                    b.HasOne("ArdaProje.Models.PT", "PT")
                        .WithMany("AppointmentRequests")
                        .HasForeignKey("PTID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArdaProje.Models.User", "User")
                        .WithMany("AppointmentRequests")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PT");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ArdaProje.Models.ClosedSlots", b =>
                {
                    b.HasOne("ArdaProje.Models.PT", null)
                        .WithMany("ClosedSlots")
                        .HasForeignKey("PTID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArdaProje.Models.User", b =>
                {
                    b.HasOne("ArdaProje.Models.PT", "PT")
                        .WithMany("Users")
                        .HasForeignKey("PTID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PT");
                });

            modelBuilder.Entity("ArdaProje.Models.PT", b =>
                {
                    b.Navigation("AppointmentRequests");

                    b.Navigation("ClosedSlots");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("ArdaProje.Models.User", b =>
                {
                    b.Navigation("AppointmentRequests");
                });
#pragma warning restore 612, 618
        }
    }
}