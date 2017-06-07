namespace Filer.EntityFrameworkCore
{
	using System;
	using System.Collections.Generic;
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
		/// Gets IQueryable of all <see cref="FileContext"/> records.
		/// </summary>
		public IQueryable<FileContext> FileContexts => this.dbContext.FileContexts.AsNoTracking();

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
		public IQueryable<File> Files => this.dbContext.Files.AsQueryable().AsNoTracking();

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

		/// <summary>
		/// Delete file permanently.
		/// </summary>
		/// <param name="fileId">Id of file to be deleted.</param>
		/// <param name="forceDeleteAllContexts">Indicate whether to detach file from all contexts 
		/// before deleting. If set to "false" and file is still associated with some contexts, 
		/// then <see cref="InvalidOperationException"/> will thrown.</param>
		public async Task DeleteFile(int fileId, bool forceDeleteAllContexts = false)
		{
			await this.DeleteFiles(new[] { fileId }, forceDeleteAllContexts);
		}

		/// <summary>
		/// Delete files permanently.
		/// </summary>
		/// <param name="fileId">Ids of files to be deleted.</param>
		/// <param name="forceDeleteAllContexts">Indicate whether to detach file from all contexts 
		/// before deleting. If set to "false" and file is still associated with some contexts, 
		/// then <see cref="InvalidOperationException"/> will thrown.</param>
		public async Task DeleteFiles(IEnumerable<int> fileId, bool forceDeleteAllContexts = false)
		{
			var files = await this.dbContext.Files
				.Include(t => t.Contexts)
				.Where(t => fileId.Contains(t.Id))
				.ToListAsync();

			foreach (var file in files)
			{
				if (file.Contexts.Any())
				{
					if (forceDeleteAllContexts)
					{
						this.dbContext.FileContexts.RemoveRange(file.Contexts);
					}
					else
					{
						throw new InvalidOperationException($"Cannot delete file '{file.Name}' because it is being used in at least 1 context.");
					}
				}

				this.dbContext.Files.Remove(file);
			}

			await this.dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Disassociates file from a context.
		/// </summary>
		/// <param name="fileId">Id of the file.</param>
		/// <param name="contexts">Context identifiers.</param>
		public async Task DetachFileFromContexts(int fileId, params string[] contexts)
		{
			var file = await this.dbContext.Files
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			foreach (var context in contexts)
			{
				var contextsToRemove = file.Contexts
					.Where(t => t.Value.Equals(context, StringComparison.OrdinalIgnoreCase))
					.ToList();

				this.dbContext.FileContexts.RemoveRange(contextsToRemove);
			}

			await this.dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Disassociates all files from specified contexts.
		/// </summary>
		/// <param name="contexts">Context identifiers.</param>
		public async Task DetachFilesFromContexts(params string[] contexts)
		{
			var contextsCsv = string.Join(",", contexts.Select(t => $"'{t}'"));
			await this.dbContext.Database.ExecuteSqlCommandAsync($"delete from FileContext where Value in ({contextsCsv})");
		}
	}
}