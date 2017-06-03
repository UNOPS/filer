namespace Filer.EntityFrameworkCore
{
	using System.IO;
	using System.Linq;
	using Filer.Core;
	using File = Filer.Core.File;

	/// <summary>
	/// Manages files in the FileStore repository.
	/// </summary>
	public class FileManager : IFileManager
	{
		private readonly FileStoreContext dbContext;

		/// <summary>
		/// Instantiates a new instance of the FileManage class.
		/// </summary>
		public FileManager(DataContext context)
		{
			this.dbContext = context.DbContext;
		}

		/// <summary>
		/// Gets file by id.
		/// </summary>
		/// <param name="id">File id.</param>
		/// <returns>File instance or null if no matching file was found.</returns>
		public File GetById(int id)
		{
			return this.dbContext.Files.AsQueryable().SingleOrDefault(f => f.Id == id);
		}

		/// <summary>
		/// Saves file in the FileStore repository.
		/// </summary>
		/// <param name="filename">Name of the file (including extension).</param>
		/// <param name="mimetype">Mime type of the file.</param>
		/// <param name="data">File's payload.</param>
		/// <param name="compressionFormat">Type of compression to apply to a file before saving it in the database.</param>
		/// <param name="owner">Owner of the file.</param>
		/// <returns>File instance.</returns>
		public File SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat, string owner)
		{
			var file = new File(owner)
			{
				Name = Path.GetFileName(filename),
				Size = data.Length,
				Extension = Path.GetExtension(filename),
				MimeType = mimetype,
				CompressionFormat = compressionFormat,
				Data = new FileData(data, compressionFormat)
			};

			this.dbContext.Files.Add(file);
			this.dbContext.SaveChanges();

			return file;
		}

		/// <summary>
		/// Returns IQueryable of all files.
		/// </summary>
		/// <returns>IQueryable of files.</returns>
		public IQueryable<File> GetAll()
		{
			return this.dbContext.Files.AsQueryable();
		}
	}
}