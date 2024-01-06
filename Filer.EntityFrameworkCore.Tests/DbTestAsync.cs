namespace Filer.EntityFrameworkCore.Tests
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
	using Xunit;

	[Collection(nameof(DatabaseCollectionFixture))]
	public class DbTestAsync
	{
		public DbTestAsync(DatabaseFixture dbFixture)
		{
			this.dbFixture = dbFixture;
		}

		private readonly DatabaseFixture dbFixture;

		[Fact]
		public async Task CanAttachToContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId, "invoice:123", "contract:321");

			var file = await fileManager.Files
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(2, file.Contexts.Count);
		}

		[Fact]
		public async Task CanCreateFile()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			Assert.NotEqual(0, fileId);
			Assert.NotNull(fileManager.GetById(fileId));
		}

		[Fact]
		public async Task CanDeleteFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId, "invoice:123", "contract:321", "order:123");

			await AssertEx.ThrowsAsync<InvalidOperationException>(async () => await fileManager.DeleteFileAsync(fileId));

			await fileManager.DeleteFileAsync(fileId, true);
		}

		[Fact]
		public async Task CanDeleteFilesInBulk()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);
			var fileId2 = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId1, "invoice:1", "contract:1", "order:3");
			await fileManager.AttachFileToContextsAsync(fileId2, "invoice:2", "contract:2", "order:3");

			await AssertEx.ThrowsAsync<InvalidOperationException>(
				async () => await fileManager.DeleteFilesAsync(
					new[]
					{
						fileId1,
						fileId2
					}));

			await fileManager.DeleteFilesAsync(
				new[]
				{
					fileId1,
					fileId2
				},
				true);
		}

		[Fact]
		public async Task CanDetachFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId, "invoice:123", "contract:321", "order:123");

			await fileManager.DetachFileFromContextsAsync(fileId, "invoice:123", "order:123");

			var file = await fileManager.FileContexts.Where(t => t.FileId == fileId).ToListAsync();

			Assert.True(file.Count == 1);
			Assert.True(file[0].Value == "contract:321");
		}

		[Fact]
		public async Task CanGetFilesByContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId, "invoice:123", "contract:321");

			var files = await fileManager.FileContexts.Where(t => t.Value == "invoice:123").ToListAsync();
			Assert.Contains(files, t => t.FileId == fileId);
		}

		[Fact]
		public async Task CanRemoveContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);
			var fileId2 = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip);

			await fileManager.AttachFileToContextsAsync(fileId1, "invoice:1", "contract:1", "user:123");
			await fileManager.AttachFileToContextsAsync(fileId2, "invoice:2", "contract:2", "user:123");

			await fileManager.DetachFilesFromContextsAsync("user:123");

			var userFilesExist = await fileManager.FileContexts.AnyAsync(t => t.Value == "user:123");
			Assert.False(userFilesExist);
		}

		[Fact]
		public async Task CanSpecifyUploader()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFileAsync("test.txt", "text/plain", Array.Empty<byte>(), CompressionFormat.GZip, 12345);

			var file = await fileManager.Files
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(12345, file.CreatedByUserId);
		}
	}
}