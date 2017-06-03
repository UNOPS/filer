namespace Filer.Core
{
	/// <summary>
	/// Represents a context where this file is being used.
	/// </summary>
	public class FileContext
	{
		/// <summary>
		/// Gets or sets if of the file.
		/// </summary>
		public int FileId { get; set; }

		/// <summary>
		/// Gets or sets an arbitrary string which represents context.
		/// </summary>
		/// <remarks>This can be an arbitrary string, which represents the context where
		/// the file is used, for example if the file is part of a contract #123 then
		/// the context can be specified as "contract:123", or if this file is part of an invoice #321
		/// then it can be "invoice:321".</remarks>
		public string Value { get; set; }
	}
}