﻿// <auto-generated />
using System;
using ArdaProje.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArdaProje.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240213122426_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArdaProje.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AppointmentID"));

                    b.Property<DateTime>("AppointmentDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PTID")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserID")
                        .HasColumnType("integer");

                    b.HasKey("AppointmentID");

                    b.HasIndex("PTID");

                    b.HasIndex("UserID");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("ArdaProje.Models.PT", b =>
                {
                    b.Property<int>("PTID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PTID"));

                    b.Property<string>("ExpertiseArea")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PTID");

                    b.ToTable("PTs");
                });

            modelBuilder.Entity("ArdaProje.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PTID")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

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
