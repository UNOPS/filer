namespace Filer.Core
{
	using System.IO;
	using System.IO.Compression;

	/// <summary>
	/// Provides GZip compression utilities.
	/// </summary>
	public static class GZip
	{
		/// <summary>
		/// Compresses the specified data into GZip format.
		/// </summary>
		/// <param name="data">Data to compress.</param>
		/// <returns>Data in GZip format.</returns>
		public static byte[] Compress(byte[] data)
		{
			using (var compressedStream = new MemoryStream())
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
			{
				zipStream.Write(data, 0, data.Length);
				return compressedStream.ToArray();
			}
		}

		/// <summary>
		/// Decompresses the specified data from GZip format.
		/// </summary>
		/// <param name="data">Data in GZip format.</param>
		/// <returns>Data in its original format.</returns>
		public static byte[] Decompress(byte[] data)
		{
			using (var compressedStream = new MemoryStream(data))
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var resultStream = new MemoryStream())
			{
				zipStream.CopyTo(resultStream);
				return resultStream.ToArray();
			}
		}
	}
}