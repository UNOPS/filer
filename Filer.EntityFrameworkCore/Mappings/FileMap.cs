﻿namespace Filer.EntityFrameworkCore.Mappings
{
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	internal class FileMap : DbEntityConfiguration<File>
	{
		public override void Configure(EntityTypeBuilder<File> entity)
		{
			entity.ToTable("File");
			entity.HasKey(t => t.Id);

			entity.Property(t => t.Id).HasColumnName("Id");
			entity.Property(t => t.Name).HasColumnName("Name").IsUnicode().HasMaxLength(255);
			entity.Property(t => t.Extension).HasColumnName("Extension").IsUnicode().HasMaxLength(20);
			entity.Property(t => t.MimeType).HasColumnName("MimeType").IsUnicode(false).HasMaxLength(100);
			entity.Property(t => t.Size).HasColumnName("Size");
			entity.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
			entity.Property(t => t.CompressionFormatId).HasColumnName("CompressionFormat");
			entity.Property(t => t.Owner).HasColumnName("Owner").HasMaxLength(50).IsUnicode(false);

			entity.HasIndex(t => t.Owner).HasName("IX_File_Owner");

			entity.Ignore(t => t.CompressionFormat);

			entity.HasOne(t => t.Data)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasMany(t => t.Contexts)
				.WithOne(t => t.File)
				.HasForeignKey(t => t.FileId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}