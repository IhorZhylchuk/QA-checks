﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QA_checks.Models;

#nullable disable

namespace QA_checks.Migrations
{
    [DbContext(typeof(ApplicationDbContex))]
    [Migration("20231021215606_Update")]
    partial class Update
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QA_checks.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("OrdersName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OrdersNumber")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("QA_checks.Models.QAchecks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CiałaObce")
                        .HasColumnType("int");

                    b.Property<string>("CiałaObceKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DataOpakowania")
                        .HasColumnType("int");

                    b.Property<string>("DataOpakowaniaKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Ekstrakt")
                        .HasColumnType("real");

                    b.Property<float>("Lepkość")
                        .HasColumnType("real");

                    b.Property<int>("MetalDetektor")
                        .HasColumnType("int");

                    b.Property<string>("MetalDetektorKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Opakowanie")
                        .HasColumnType("int");

                    b.Property<string>("OpakowanieKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<long>("OrdersNumber")
                        .HasColumnType("bigint");

                    b.Property<int>("Pasteryzacja")
                        .HasColumnType("int");

                    b.Property<string>("PasteryzacjaKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Ph")
                        .HasColumnType("real");

                    b.Property<int>("Receptura")
                        .HasColumnType("int");

                    b.Property<string>("RecepturaKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Temperatura")
                        .HasColumnType("real");

                    b.Property<string>("TestKomentarz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TestWodny")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("QAchecks");
                });
#pragma warning restore 612, 618
        }
    }
}
