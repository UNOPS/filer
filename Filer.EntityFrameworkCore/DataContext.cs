namespace Filer.EntityFrameworkCore
{
	using Microsoft.EntityFrameworkCore;

	/// <summary>
	/// Represents a single unit of work.
	/// </summary>
	public class DataContext
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
	}
}