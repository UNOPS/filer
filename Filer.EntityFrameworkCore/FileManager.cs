namespace Filer.EntityFrameworkCore
{
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
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
		public async Task<File> GetById(int id)
		{
			return await this.dbContext.Files.FindAsync(id);
		}

		/// <summary>
		/// Saves file in the FileStore repository.
		/// </summary>
		/// <param name="filename">Name of the file (including extension).</param>
		/// <param name="mimetype">Mime type of the file.</param>
		/// <param name="data">File's payload.</param>
		/// <param name="compressionFormat">Type of compression to apply to a file before saving it in the database.</param>
		/// <param name="userId">Id of the user who will be listed as file's creator (<see cref="File.CreatedByUserId"/>).</param>
		/// <returns>Id of the newly created file.</returns>
		public async Task<int> SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat, int? userId = null)
		{
			var file = new File(userId)
			{
				Name = Path.GetFileName(filename),
				Size = data.Length,
				Extension = Path.GetExtension(filename),
				MimeType = mimetype,
				CompressionFormat = compressionFormat,
				Data = new FileData(data, compressionFormat)
			};

			this.dbContext.Files.Add(file);
			await this.dbContext.SaveChangesAsync();

			return file.Id;
		}

		/// <summary>
		/// Returns IQueryable of all files.
		/// </summary>
		/// <returns>IQueryable of files.</returns>
		public IQueryable<File> GetAll()
		{
			return this.dbContext.Files.AsQueryable();
		}

		/// <summary>
		/// Attaches file to a context. All previous contexts are kept intact.
		/// </summary>
		/// <remarks>This operation basically adds a new <see cref="FileContext"/> to the 
		/// <see cref="File.Contexts"/> collection.  If the file was attached to the context 
		/// already, then nothing happens.</remarks>
		/// <param name="fileId">If of the file.</param>
		/// <param name="contexts">List of contexts to attach to.</param>
		public async Task AttachFileToContexts(int fileId, params string[] contexts)
		{
			var file = await this.dbContext.Files
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			foreach (var context in contexts)
			{
				file.AttachToContext(context);
			}

			await this.dbContext.SaveChangesAsync();
		}
	}
}