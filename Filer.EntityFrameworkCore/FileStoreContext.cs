namespace Filer.EntityFrameworkCore
{
	using Filer.Core;
	using Filer.EntityFrameworkCore.Mappings;
	using Microsoft.EntityFrameworkCore;

	internal class FileStoreContext : DbContext
	{
		public FileStoreContext(DbContextOptions options)
			: base(options)
		{
		}

		public DbSet<File> Files { get; set; }
		public DbSet<FileData> FileData { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.AddConfiguration(new FileMap());
			modelBuilder.AddConfiguration(new FileDataMap());
		}
	}
}