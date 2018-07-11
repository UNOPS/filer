﻿namespace Filer.EntityFrameworkCore
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

		/// <inheritdoc />
		public IQueryable<FileContext> FileContexts => this.dbContext.FileContexts.AsNoTracking();

		/// <inheritdoc />
		public async Task<File> GetByIdAsync(int id)
		{
			return await this.dbContext.Files.FindAsync(id);
		}

		/// <inheritdoc />
		public File GetById(int id)
		{
			return this.dbContext.Files.Find(id);
		}

		/// <inheritdoc />
		public int SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat, int? userId = null)
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
			this.dbContext.SaveChanges();

			return file.Id;
		}

		/// <inheritdoc />
		public async Task<int> SaveFileAsync(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat, int? userId = null)
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

		/// <inheritdoc />
		public IQueryable<File> Files => this.dbContext.Files.AsQueryable().AsNoTracking();

		/// <inheritdoc />
		public async Task AttachFileToContextsAsync(int fileId, params string[] contexts)
		{
			var file = await this.dbContext.Files
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			if (file == null)
			{
				throw new NullReferenceException($"File [id:{fileId}] does not exists");
			}

			foreach (var context in contexts)
			{
				file.AttachToContext(context);
			}

			await this.dbContext.SaveChangesAsync();
		}

		/// <inheritdoc />
		public async Task DeleteFileAsync(int fileId, bool forceDeleteAllContexts = false)
		{
			await this.DeleteFilesAsync(new[] { fileId }, forceDeleteAllContexts);
		}

		/// <inheritdoc />
		public void DeleteFile(int fileId, bool forceDeleteAllContexts = false)
		{
			this.DeleteFiles(new[] { fileId }, forceDeleteAllContexts);
		}

		/// <inheritdoc />
		public void DeleteFiles(IEnumerable<int> fileId, bool forceDeleteAllContexts = false)
		{
			var files = this.dbContext.Files
				.Include(t => t.Contexts)
				.Where(t => fileId.Contains(t.Id))
				.ToList();

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

			this.dbContext.SaveChanges();
		}

		/// <inheritdoc />
		public async Task DeleteFilesAsync(IEnumerable<int> fileId, bool forceDeleteAllContexts = false)
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

		/// <inheritdoc />
		public void DetachFileFromContexts(int fileId, params string[] contexts)
		{
			var file = this.dbContext.Files
				.Include(t => t.Contexts)
				.SingleOrDefault(t => t.Id == fileId);

			if (file == null)
			{
				throw new NullReferenceException($"File [id:{fileId}] does not exists");
			}

			foreach (var context in contexts)
			{
				var contextsToRemove = file.Contexts
					.Where(t => t.Value.Equals(context, StringComparison.OrdinalIgnoreCase))
					.ToList();

				this.dbContext.FileContexts.RemoveRange(contextsToRemove);
			}

			this.dbContext.SaveChanges();
		}

		/// <inheritdoc />
		public async Task DetachFileFromContextsAsync(int fileId, params string[] contexts)
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

		/// <inheritdoc />
		public void Dispose()
		{
			this.dbContext?.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public void AttachFileToContexts(int fileId, params string[] contexts)
		{
			var file = this.dbContext.Files
				.Include(t => t.Contexts)
				.SingleOrDefault(t => t.Id == fileId);

			if (file == null)
			{
				throw new NullReferenceException($"File [id:{fileId}] does not exists");
			}

			foreach (var context in contexts)
			{
				file.AttachToContext(context);
			}

			this.dbContext.SaveChanges();
		}

		/// <summary>
		/// Disassociates all files from specified contexts.
		/// </summary>
		/// <param name="contexts">Context identifiers.</param>
		public int DetachFilesFromContexts(params string[] contexts)
		{
			var contextsCsv = string.Join(",", contexts.Select(t => $"'{t}'"));
			return this.dbContext.Database.ExecuteSqlCommand(new RawSqlString($"delete from [FileContext] where [Value] in ({contextsCsv})"));
		}

		/// <summary>
		/// Disassociates all files from specified contexts.
		/// </summary>
		/// <param name="contexts">Context identifiers.</param>
		public async Task<int> DetachFilesFromContextsAsync(params string[] contexts)
		{
			var contextsCsv = string.Join(",", contexts.Select(t => $"'{t}'"));
			return await this.dbContext.Database.ExecuteSqlCommandAsync(new RawSqlString($"delete from [FileContext] where [Value] in ({contextsCsv})"));
		}
	}
}