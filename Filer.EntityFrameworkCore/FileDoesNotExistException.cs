namespace Filer.EntityFrameworkCore;

using System;

/// <summary>
/// Indicates that a file does not exist in the database.
/// </summary>
public class FileDoesNotExistException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="FileDoesNotExistException"/> class.
	/// </summary>
	/// <param name="fileId">Id of the file.</param>
	internal FileDoesNotExistException(int fileId) : base($"File #{fileId} does not exist.")
	{
	}
}