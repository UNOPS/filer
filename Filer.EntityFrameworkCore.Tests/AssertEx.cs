﻿namespace Filer.EntityFrameworkCore.Tests
{
	using System;
	using System.Threading.Tasks;

	// https://msdn.microsoft.com/en-us/magazine/dn818493.aspx
	public static class AssertEx
	{
		public static async Task<TException> ThrowsAsync<TException>(
			Func<Task> action,
			bool allowDerivedTypes = true) where TException : Exception
		{
			try
			{
				await action();
			}
			catch (Exception ex)
			{
				if (allowDerivedTypes && ex is not TException)
				{
					throw new Exception(
						$"Delegate threw exception of type {ex.GetType().Name}, " +
						$"but {typeof(TException).Name} or a derived type was expected.",
						ex);
				}

				if (!allowDerivedTypes && ex.GetType() != typeof(TException))
				{
					throw new Exception(
						$"Delegate threw exception of type {ex.GetType().Name}, " +
						$"but {typeof(TException).Name} was expected.",
						ex);
				}

				return (TException)ex;
			}

			throw new Exception($"Delegate did not throw expected exception {typeof(TException).Name}.");
		}
	}
}