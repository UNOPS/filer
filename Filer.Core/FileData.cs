namespace Filer.Core
{
	using System;

	/// <summary>
	/// Represents the payload of the file stored in the FileStore repository.
	/// </summary>
	public class FileData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileData"/> class.
		/// </summary>
		protected FileData()
		{
		}

		internal FileData(byte[] data, CompressionFormat compressionFormat)
		{
			switch (compressionFormat)
			{
				case CompressionFormat.None:
					this.Data = data;
					break;

				case CompressionFormat.GZip:
					this.Data = GZip.Compress(data);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(compressionFormat));
			}
		}

		/// <summary>
		/// Gets or sets the file's payload.
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// Gets or sets the file's ID.
		/// </summary>
		public int FileId { get; set; }
	}
}