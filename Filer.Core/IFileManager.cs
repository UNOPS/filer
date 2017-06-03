namespace Filer.Core
{
	using System.Linq;

	/// <summary>
	/// Manages files in the FileStore repository.
	/// </summary>
	public interface IFileManager
	{
		/// <summary>
		/// Gets file by id.
		/// </summary>
		/// <param name="id">File id.</param>
		/// <returns>File instance or null if no matching file was found.</returns>
		File GetById(int id);

		/// <summary>
		/// Saves file in the FileStore repository.
		/// </summary>
		/// <param name="filename">Name of the file (including extension).</param>
		/// <param name="mimetype">Mime type of the file.</param>
		/// <param name="data">File's payload.</param>
		/// <param name="compressionFormat">Type of compression to apply to a file before saving it in the database.</param>
		/// <returns>File instance.</returns>
		File SaveFile(string filename, string mimetype, byte[] data, CompressionFormat compressionFormat);
		
		/// <summary>
		/// Returns IQueryable of all files.
		/// </summary>
		/// <returns>IQueryable of files.</returns>
		IQueryable<File> GetAll();
	}
}