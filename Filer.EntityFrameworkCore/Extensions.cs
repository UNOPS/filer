namespace Filer.EntityFrameworkCore
{
	using System;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;

	public static class Extensions
	{
		internal static void AddConfiguration<TEntity>(
			this ModelBuilder modelBuilder,
			DbEntityConfiguration<TEntity> entityConfiguration) where TEntity : class
		{
			modelBuilder.Entity<TEntity>(entityConfiguration.Configure);
		}

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
	}
}