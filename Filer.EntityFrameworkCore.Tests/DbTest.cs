namespace Filer.EntityFrameworkCore.Tests
{
	using System;
	using System.Linq;
	using Filer.Core;
	using Microsoft.EntityFrameworkCore;
	using Xunit;

	[Collection(nameof(DatabaseCollectionFixture))]
	public class DbTest
	{
		public DbTest(DatabaseFixture dbFixture)
		{
			this.dbFixture = dbFixture;
		}

		private readonly DatabaseFixture dbFixture;

		[Fact]
		public async void CanAttachToContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321");

			var file = await fileManager.Files
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(2, file.Contexts.Count);
		}

		[Fact]
		public async void CanCreateFile()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			Assert.NotEqual(0, fileId);
			Assert.NotNull(fileManager.GetById(fileId));
		}

		[Fact]
		public async void CanDeleteFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321", "order:123");

			await AssertEx.ThrowsAsync<InvalidOperationException>(async () => await fileManager.DeleteFile(fileId));

			await fileManager.DeleteFile(fileId, true);
		}

		[Fact]
		public async void CanDeleteFilesInBulk()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);
			var fileId2 = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId1, "invoice:1", "contract:1", "order:3");
			await fileManager.AttachFileToContexts(fileId2, "invoice:2", "contract:2", "order:3");

			await AssertEx.ThrowsAsync<InvalidOperationException>(async () => await fileManager.DeleteFiles(new[] { fileId1, fileId2 }));

			await fileManager.DeleteFiles(new[] { fileId1, fileId2 }, true);
		}

		[Fact]
		public async void CanDetachFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321", "order:123");

			await fileManager.DetachFileFromContexts(fileId, "invoice:123", "order:123");

			var file = await fileManager.FileContexts.Where(t => t.FileId == fileId).ToListAsync();

			Assert.True(file.Count == 1);
			Assert.True(file[0].Value == "contract:321");
		}

		[Fact]
		public async void CanGetFilesByContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321");

			var files = await fileManager.FileContexts.Where(t => t.Value == "invoice:123").ToListAsync();
			Assert.True(files.Any(t => t.FileId == fileId));
		}

		[Fact]
		public async void CanSpecifyUploader()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip, 12345);

			var file = await fileManager.Files
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(12345, file.CreatedByUserId);
		}

		[Fact]
		public async void CanRemoveContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);
			var fileId2 = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			await fileManager.AttachFileToContexts(fileId1, "invoice:1", "contract:1", "user:123");
			await fileManager.AttachFileToContexts(fileId2, "invoice:2", "contract:2", "user:123");

			await fileManager.DetachFilesFromContexts("user:123");

			var userFilesExist = await fileManager.FileContexts.AnyAsync(t => t.Value == "user:123");
			Assert.False(userFilesExist);
		}
	}
}