namespace Filer.EntityFrameworkCore
{
	using System.Linq;
	using System.Threading.Tasks;
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;

	/// <summary>
	/// Extensions for <see cref="Filer.EntityFrameworkCore"/>
	/// </summary>
	public static class Extensions
	{
		internal static File SingleOrException(this IQueryable<File> source, int fileId)
		{
			var file = source.SingleOrDefault(t => t.Id == fileId);

			if (file == null)
			{
				throw new FileDoesNotExistException(fileId);
			}

			return file;
		}

		internal static async Task<File> SingleOrExceptionAsync(this IQueryable<File> source, int fileId)
		{
			var file = await source.SingleOrDefaultAsync(t => t.Id == fileId);

			if (file == null)
			{
				throw new FileDoesNotExistException(fileId);
			}

			return file;
		}
	}
}