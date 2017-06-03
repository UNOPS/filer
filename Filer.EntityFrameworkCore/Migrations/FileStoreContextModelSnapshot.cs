namespace Filer.EntityFrameworkCore.Migrations
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Infrastructure;
	using Microsoft.EntityFrameworkCore.Metadata;

	[DbContext(typeof(FileStoreContext))]
	partial class FileStoreContextModelSnapshot : ModelSnapshot
	{
		protected override void BuildModel(ModelBuilder modelBuilder)
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

				b.ToTable("File");
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