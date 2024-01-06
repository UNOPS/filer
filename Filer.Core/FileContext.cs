namespace Filer.Core
{
	using System;

	/// <summary>
	/// Represents a context where this file is being used.
	/// </summary>
	public class FileContext
	{
		/// <summary>
		/// Max length for the <see cref="Value"/>.
		/// </summary>
		public const int ValueMaxLength = 50;

		protected FileContext()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileContext"/> class.
		/// </summary>
		/// <param name="fileId"></param>
		/// <param name="value"></param>
		internal FileContext(int fileId, string value)
		{
			if (value?.Length > ValueMaxLength)
			{
				throw new ArgumentException($"Value must be at most {ValueMaxLength} characters long.", nameof(value));
			}

			this.Value = value;
			this.FileId = fileId;
		}

		/// <summary>
		/// Gets or sets file associated with this context.
		/// </summary>
		public virtual File File { get; protected set; }

		/// <summary>
		/// Gets or sets if of the file.
		/// </summary>
		public int FileId { get; protected set; }

		/// <summary>
		/// Gets or sets an arbitrary string which represents context.
		/// </summary>
		/// <remarks>This can be an arbitrary string, which represents the context where
		/// the file is used, for example if the file is part of a contract #123 then
		/// the context can be specified as "contract:123", or if this file is part of an invoice #321
		/// then it can be "invoice:321".</remarks>
		public string Value { get; protected set; }
	}
}