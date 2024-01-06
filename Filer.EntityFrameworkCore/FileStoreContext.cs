namespace Filer.EntityFrameworkCore
{
	using Filer.Core;
	using Filer.EntityFrameworkCore.Mappings;
	using Microsoft.EntityFrameworkCore;

	internal class FileStoreContext(DbContextOptions options) : DbContext(options)
	{
		private const string DefaultConnectionString =
			"Server=(localdb)\\mssqllocaldb;Database=filer;Trusted_Connection=True;MultipleActiveResultSets=true";

		public FileStoreContext()
			: this(new DbContextOptionsBuilder().UseSqlServer(DefaultConnectionString).Options)
		{
		}

		public DbSet<FileContext> FileContexts { get; set; }
		public DbSet<FileData> FileData { get; set; }
		public DbSet<File> Files { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new FileMap());
			modelBuilder.ApplyConfiguration(new FileDataMap());
			modelBuilder.ApplyConfiguration(new FileContextMap());
		}
	}
}