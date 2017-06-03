namespace Filer.EntityFrameworkCore.Mappings
{
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	internal class FileDataMap : DbEntityConfiguration<FileData>
	{
		public override void Configure(EntityTypeBuilder<FileData> entity)
		{
			entity.ToTable("File");
			entity.HasKey(t => t.FileId);

			entity.Property(t => t.FileId).HasColumnName("Id");
			entity.Property(t => t.Data).HasColumnName("Data");
		}
	}
}