﻿// <auto-generated />
using System;
using Filer.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Filer.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(FileStoreContext))]
    partial class FileStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Filer.Core.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id")
                        .UseIdentityColumn();

                    b.Property<byte>("CompressionFormatId")
                        .HasColumnType("tinyint")
                        .HasColumnName("CompressionFormat");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int")
                        .HasColumnName("CreatedByUserId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedOn");

                    b.Property<string>("Extension")
                        .HasMaxLength(20)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("Extension");

                    b.Property<string>("MimeType")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("MimeType");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("Name");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("Size");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId")
                        .HasDatabaseName("IX_File_CreatedByUserId");

                    b.ToTable("File");
                });

            modelBuilder.Entity("Filer.Core.FileContext", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int")
                        .HasColumnName("FileId");

                    b.Property<string>("Value")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("Value");

                    b.HasKey("FileId", "Value");

                    b.HasIndex("Value", "FileId")
                        .IsUnique();

                    b.ToTable("FileContext");
                });

            modelBuilder.Entity("Filer.Core.FileData", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    b.Property<byte[]>("Data")
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("Data");

                    b.HasKey("FileId");

                    b.ToTable("FileData");
                });

            modelBuilder.Entity("Filer.Core.FileContext", b =>
                {
                    b.HasOne("Filer.Core.File", "File")
                        .WithMany("Contexts")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("Filer.Core.FileData", b =>
                {
                    b.HasOne("Filer.Core.File", null)
                        .WithOne("Data")
                        .HasForeignKey("Filer.Core.FileData", "FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Filer.Core.File", b =>
                {
                    b.Navigation("Contexts");

                    b.Navigation("Data");
                });
#pragma warning restore 612, 618
        }
    }
}
