namespace Filer.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Represents a file stored in the FileStore repository.
	/// </summary>
	public class File
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="File"/> class.
		/// </summary>
		/// <param name="createdByUserId">Id of the user who created the file.</param>
		internal File(int? createdByUserId = null)
		{
			this.CreatedByUserId = createdByUserId;
			this.CreatedOn = DateTime.UtcNow;
		}

		/// <summary>
		/// Initializes a new instance of the File class.
		/// </summary>
		internal File()
			: this(null)
		{
		}

		/// <summary>
		/// Gets or sets compression format in which the file is stored.
		/// </summary>
		public CompressionFormat CompressionFormat
		{
			get => (CompressionFormat)this.CompressionFormatId;
			set => this.CompressionFormatId = (byte)value;
		}

		/// <summary>
		/// Gets or sets byte representation of the compression format in which the file is stored.
		/// </summary>
		public byte CompressionFormatId { get; set; }

		/// <summary>
		/// Gets all contexts where this file is used.
		/// </summary>
		public virtual ICollection<FileContext> Contexts { get; protected set; }

		/// <summary>
		/// Gets of sets id of user who created this file.
		/// </summary>
		/// <remarks>This property can be an arbitrary number, and in itself doesn't
		/// have any meaning or function within <see cref="Filer"/> codebase.</remarks>
		public int? CreatedByUserId { get; protected set; }

		/// <summary>
		/// Gets or sets the date when the file was uploaded.
		/// </summary>
		public DateTime CreatedOn { get; protected set; }

		/// <summary>
		/// Gets or sets the file's payload.
		/// </summary>
		/// <remarks>This is a lazy loaded property, so accessing this property for the first
		/// time will result in a database call.</remarks>
		public virtual FileData Data { get; set; }

		/// <summary>
		/// Gets or sets the extension.
		/// </summary>
		/// <remarks>Extensions are stored with the preceding period (".") character.</remarks>
		public string Extension { get; set; }

		/// <summary>
		/// Gets or sets the file id.
		/// </summary>
		public int Id { get; protected set; }

		/// <summary>
		/// Gets or sets the mime type.
		/// </summary>
		public string MimeType { get; set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets size of the file in bytes. The size is always that of the original
		/// file, not of the compressed version in case it was compressed for database storage.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Decompresses file data and returns the original payload.
		/// </summary>
		/// <returns>Byte array.</returns>
		/// <remarks>In case no compression has been applied, then the file data is returned 
		/// directly without performing any decompression.</remarks>
		public byte[] DecompressFile()
		{
			switch (this.CompressionFormat)
			{
				case CompressionFormat.None:
					return this.Data.Data;

				case CompressionFormat.GZip:
					return GZip.Decompress(this.Data.Data);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Disassociates file from a context.
		/// </summary>
		/// <param name="context">Context identifier.</param>
		internal void DetachFromContext(string context)
		{
			var contextsToRemove = this.Contexts
				.Where(t => t.Value.Equals(context, StringComparison.OrdinalIgnoreCase))
				.ToList();

			foreach (var c in contextsToRemove)
			{
				this.Contexts.Remove(c);
			}
		}

		/// <summary>
		/// Attaches file to a context. All previous contexts are kept intact.
		/// </summary>
		/// <remarks>This operation basically adds a new <see cref="FileContext"/> to the <see cref="Contexts"/> collection.
		/// If the file was attached to the context already, then nothing happens.</remarks>
		/// <param name="context">Context identifier.</param>
		internal void AttachToContext(string context)
		{
			var exists = this.Contexts.Any(t => t.Value.Equals(context, StringComparison.OrdinalIgnoreCase));

			if (!exists)
			{
				this.Contexts.Add(new FileContext(this.Id, context));
			}
		}
	}
}