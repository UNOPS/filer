namespace Filer.EntityFrameworkCore
{
	using System;
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
		/// <returns>File instance.</returns>
		public File SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat)
		{
			var file = new File
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
		/// Saves file in a temporary storage. Temporary storage will periodically be cleared.
		/// </summary>
		/// <param name="filename">Name of the file (including extension).</param>
		/// <param name="mimetype">Mime type of the file.</param>
		/// <param name="data">File's payload.</param>
		/// <param name="uploaderUserId">Id of the user (from UserManagement database) who has uploaded the document.</param>
		/// <param name="compressionFormat">Type of compression to apply to a file before saving it in the database.</param>
		/// <returns>File instance.</returns>
		public File SaveFileTemporarily(string filename, string mimetype, byte[] data, int uploaderUserId, CompressionFormat compressionFormat)
		{
			return this.SaveFile(filename, mimetype, data, compressionFormat);
		}

		/// <summary>
		/// Decompresses file data and returns the original payload.
		/// </summary>
		/// <param name="file">File instance.</param>
		/// <returns>Byte array.</returns>
		/// <remarks>In case no compression has been applied, then the file data is returned 
		/// directly without performing any decompression.</remarks>
		public byte[] DecompressFile(File file)
		{
			switch (file.CompressionFormat)
			{
				case CompressionFormat.None:
					return file.Data.Data;

				case CompressionFormat.GZip:
					return GZip.Compress(file.Data.Data);

				default:
					throw new ArgumentOutOfRangeException();
			}
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
