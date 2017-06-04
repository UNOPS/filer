using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Filer.EntityFrameworkCore;

namespace Filer.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(FileStoreContext))]
    [Migration("20170604105611_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Filer.Core.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<byte>("CompressionFormatId")
                        .HasColumnName("CompressionFormat");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnName("CreatedByUserId");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnName("CreatedOn");

                    b.Property<string>("Extension")
                        .HasColumnName("Extension")
                        .HasMaxLength(20)
                        .IsUnicode(true);

                    b.Property<string>("MimeType")
                        .HasColumnName("MimeType")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasMaxLength(255)
                        .IsUnicode(true);

                    b.Property<long>("Size")
                        .HasColumnName("Size");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId")
                        .HasName("IX_File_CreatedByUserId");

                    b.ToTable("File");
                });

            modelBuilder.Entity("Filer.Core.FileContext", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnName("FileId");

                    b.Property<string>("Value")
                        .HasColumnName("Value")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("FileId", "Value");

                    b.ToTable("FileContext");
                });

            modelBuilder.Entity("Filer.Core.FileData", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnName("Id");

                    b.Property<byte[]>("Data")
                        .HasColumnName("Data");

                    b.HasKey("FileId");

                    b.ToTable("FileData");
                });

            modelBuilder.Entity("Filer.Core.FileContext", b =>
                {
                    b.HasOne("Filer.Core.File", "File")
                        .WithMany("Contexts")
                        .HasForeignKey("FileId");
                });

            modelBuilder.Entity("Filer.Core.FileData", b =>
                {
                    b.HasOne("Filer.Core.File")
                        .WithOne("Data")
                        .HasForeignKey("Filer.Core.FileData", "FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
