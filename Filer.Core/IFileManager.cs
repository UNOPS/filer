namespace Filer.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Manages files in the FileStore repository.
	/// </summary>
	public interface IFileManager
	{
		/// <summary>
		/// Gets IQueryable of all <see cref="FileContext"/> records.
		/// </summary>
		IQueryable<FileContext> FileContexts { get; }

		/// <summary>
		/// Returns IQueryable of all files.
		/// </summary>
		/// <returns>IQueryable of files.</returns>
		IQueryable<File> Files { get; }

		/// <summary>
		/// Attaches file to a context. All previous contexts are kept intact.
		/// </summary>
		/// <remarks>This operation basically adds a new <see cref="FileContext"/> to the 
		/// <see cref="File.Contexts"/> collection.  If the file was attached to the context 
		/// already, then nothing happens.</remarks>
		/// <param name="fileId">If of the file.</param>
		/// <param name="contexts">List of contexts to attach to.</param>
		Task AttachFileToContexts(int fileId, params string[] contexts);

		/// <summary>
		/// Delete file permanently.
		/// </summary>
		/// <param name="fileId">Id of file to be deleted.</param>
		/// <param name="forceDeleteAllContexts">Indicate whether to detach file from all contexts 
		/// before deleting. If set to "false" and file is still associated with some contexts, 
		/// then <see cref="InvalidOperationException"/> will thrown.</param>
		Task DeleteFile(int fileId, bool forceDeleteAllContexts = false);

		/// <summary>
		/// Delete files permanently.
		/// </summary>
		/// <param name="fileId">Ids of files to be deleted.</param>
		/// <param name="forceDeleteAllContexts">Indicate whether to detach file from all contexts 
		/// before deleting. If set to "false" and file is still associated with some contexts, 
		/// then <see cref="InvalidOperationException"/> will thrown.</param>
		Task DeleteFiles(IEnumerable<int> fileId, bool forceDeleteAllContexts = false);

		/// <summary>
		/// Disassociates file from a context.
		/// </summary>
		/// <param name="fileId">Id of the file.</param>
		/// <param name="contexts">Context identifiers.</param>
		Task DetachFileFromContexts(int fileId, params string[] contexts);

		/// <summary>
		/// Gets file by id.
		/// </summary>
		/// <param name="id">File id.</param>
		/// <returns>File instance or null if no matching file was found.</returns>
		Task<File> GetById(int id);

		/// <summary>
		/// Saves file in the FileStore repository.
		/// </summary>
		/// <param name="filename">Name of the file (including extension).</param>
		/// <param name="mimetype">Mime type of the file.</param>
		/// <param name="data">File's payload.</param>
		/// <param name="compressionFormat">Type of compression to apply to a file before saving it in the database.</param>
		/// <param name="userId">Id of the user who will be listed as file's creator (<see cref="File.CreatedByUserId"/>).</param>
		/// <returns>Id of the newly created file.</returns>
		Task<int> SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat, int? userId = null);
	}
}