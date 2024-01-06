namespace Filer.Core
{
	using System.IO;

	/// <summary>
	/// Provides methods to convert stream to a byte array and vice-versa.
	/// </summary>
	internal class StreamUtilities
	{
		/// <summary>
		/// Retrieves byte array as a read-only MemoryStream.
		/// </summary>
		/// <param name="data">Byte array.</param>
		/// <returns>MemoryStream instance.</returns>
		public static MemoryStream GetReadonlyStream(byte[] data)
		{
			return new MemoryStream(data);
		}

		/// <summary>
		/// Reads stream into a byte array.
		/// </summary>
		/// <param name="input">Stream instance.</param>
		/// <returns>Byte array.</returns>
		public static byte[] ReadFully(Stream input)
		{
			using (var ms = new MemoryStream())
			{
				input.CopyTo(ms);
				return ms.ToArray();
			}
		}
	}
}