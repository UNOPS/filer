namespace Filer.Core
{
	/// <summary>
	/// Describes different types of compressions that can be applied to a file.
	/// </summary>
	public enum CompressionFormat : byte
	{
		/// <summary>
		/// Indicates that the file isn't compressed.
		/// </summary>
		None = 0,

		/// <summary>
		/// GZip compression.
		/// </summary>
		GZip = 1
	}
}