namespace Filer.EntityFrameworkCore
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;

	/// <summary>
	/// Represents a single unit of work.
	/// </summary>
	public sealed class DataContext : IDisposable
	{
		/// <summary>
		/// Instantiates a new instance of the DataContext class.
		/// </summary>
		public DataContext(DbContextOptions options)
		{
			this.DbContext = new FileStoreContext(options);
		}

		// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
		internal FileStoreContext DbContext { get; private set; }

		/// <inheritdoc />
		public void Dispose()
		{
			if (this.DbContext != null)
			{
				this.DbContext.Dispose();
				this.DbContext = null;
			}
		}

		/// <summary>
		/// Runs <see cref="RelationalDatabaseFacadeExtensions.Migrate"/> on the underlying <see cref="Microsoft.EntityFrameworkCore.DbContext"/>,
		/// thus making sure database exists and all migrations are run.
		/// </summary>
		public void MigrateDatabase()
		{
			this.DbContext.Database.Migrate();
		}

		/// <summary>
		/// Runs <see cref="RelationalDatabaseFacadeExtensions.MigrateAsync"/> on the underlying <see cref="Microsoft.EntityFrameworkCore.DbContext"/>,
		/// thus making sure database exists and all migrations are run.
		/// </summary>
		public Task MigrateDatabaseAsync()
		{
			return this.DbContext.Database.MigrateAsync();
		}
	}
}