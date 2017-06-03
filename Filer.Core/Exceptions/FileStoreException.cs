namespace Filer.Core.Exceptions
{
	using System;

	/// <summary>
	/// Base class for exceptions thrown from this library.
	/// </summary>
	public class FileStoreException : Exception
	{
		/// <summary>
		/// Intializes a new instance of the FileStoreException class.
		/// </summary>
		public FileStoreException()
		{
		}

		/// <summary>
		/// Intializes a new instance of the FileStoreException class.
		/// </summary>
		public FileStoreException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Intializes a new instance of the FileStoreException class.
		/// </summary>
		public FileStoreException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}