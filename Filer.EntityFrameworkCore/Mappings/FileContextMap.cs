namespace Filer.EntityFrameworkCore.Mappings
{
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	internal class FileContextMap : IEntityTypeConfiguration<FileContext>
	{
		public void Configure(EntityTypeBuilder<FileContext> entity)
		{
			entity.ToTable("FileContext");
			entity.HasKey(
				t => new
				{
					t.FileId,
					t.Value
				});

			entity.Property(t => t.FileId).HasColumnName("FileId");
			entity.Property(t => t.Value).HasColumnName("Value").HasMaxLength(FileContext.ValueMaxLength).IsUnicode(false);

			entity.HasIndex(
					t => new
					{
						t.Value,
						t.FileId
					})
				.IsUnique();
		}
	}
}