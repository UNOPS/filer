namespace Filer.Core
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a file stored in the FileStore repository.
	/// </summary>
	public class File
	{
		/// <summary>
		/// Max length for the <see cref="Owner"/> property.
		/// </summary>
		public const int OwnerMaxLength = 50;

		/// <summary>
		/// Initializes a new instance of the <see cref="File"/> class.
		/// </summary>
		/// <param name="owner"></param>
		public File(string owner)
			: this()
		{
			if (owner?.Length > OwnerMaxLength)
			{
				throw new ArgumentException($"Owner must be at most {OwnerMaxLength} characters long.", nameof(owner));
			}

			this.Owner = owner;
		}

		public File()
		{
			this.CreatedOn = DateTime.UtcNow;
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
		/// Gets of sets name of owner to whom this file belongs.
		/// </summary>
		/// <remarks>This property can be an arbitrary string.</remarks>
		public string Owner { get; protected set; }

		/// <summary>
		/// Gets or sets size of the file in bytes. The size is always that of the original
		/// file, not of the compressed version in case it was compressed for database storage.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Gets all contexts where this file is used.
		/// </summary>
		public virtual ICollection<FileContext> Contexts { get; protected set; }

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
	}
}