﻿// <auto-generated />
using System;
using Backend_Example.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend_Example.Migrations
{
    [DbContext(typeof(PotluckDb))]
    [Migration("20240920143224_AddBacklinks")]
    partial class AddBacklinks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backend_Example.Database.House", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Houses");
                });

            modelBuilder.Entity("Backend_Example.Database.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CookingPoints")
                        .HasColumnType("int");

                    b.Property<int>("EuroCents")
                        .HasColumnType("int");

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Backend_Example.Database.TransactionUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.HasIndex("UserId");

                    b.ToTable("TransactionUsers");
                });

            modelBuilder.Entity("Backend_Example.Database.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AtHomeStatus")
                        .HasColumnType("int");

                    b.Property<string>("Diet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EatingTotalPeople")
                        .HasColumnType("int");

                    b.Property<int>("HouseId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HouseId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Backend_Example.Database.Transaction", b =>
                {
                    b.HasOne("Backend_Example.Database.House", "House")
                        .WithMany("Transactions")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("House");
                });

            modelBuilder.Entity("Backend_Example.Database.TransactionUser", b =>
                {
                    b.HasOne("Backend_Example.Database.Transaction", "Transaction")
                        .WithMany("TransactionUsers")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend_Example.Database.User", "User")
                        .WithMany("TransactionUsers")
                        .HasForeignKey("UserId");

                    b.Navigation("Transaction");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Backend_Example.Database.User", b =>
                {
                    b.HasOne("Backend_Example.Database.House", "House")
                        .WithMany("Users")
                        .HasForeignKey("HouseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("House");
                });

            modelBuilder.Entity("Backend_Example.Database.House", b =>
                {
                    b.Navigation("Transactions");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Backend_Example.Database.Transaction", b =>
                {
                    b.Navigation("TransactionUsers");
                });

            modelBuilder.Entity("Backend_Example.Database.User", b =>
                {
                    b.Navigation("TransactionUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
