namespace Filer.EntityFrameworkCore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;

	public static class Extensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		internal static File SingleOrException(this IQueryable<File> source, int fileId)
		{
			var file = source.SingleOrDefault(t => t.Id == fileId);

			if (file == null)
			{
				throw new NullReferenceException($"File #{fileId} does not exist.");
			}

			return file;
		}

		internal static async Task<File> SingleOrExceptionAsync(this IQueryable<File> source, int fileId)
		{
			var file = await source.SingleOrDefaultAsync(t => t.Id == fileId);

			if (file == null)
			{
				throw new NullReferenceException($"File #{fileId} does not exist.");
			}

			return file;
		}
		
		/// <summary>
		/// Change entity class name to lower case.
		/// </summary>
		/// <param name="modelBuilder"></param>
		public static void ChangeTablesAndColumnsNamesToLowerCase(this ModelBuilder modelBuilder)
		{
			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				if (entity.GetTableName() == null)
				{
					continue;
				}

				entity.SetTableName(entity.GetTableName()?.ToLower());
				foreach (var p in entity.GetProperties())
				{
					p.SetColumnName(p.GetColumnName().ToLower());
				}
			}
		}

	}
}