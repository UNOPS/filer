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
		public void CanAttachToContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321");

			var file = fileManager.Files
				.Include(t => t.Contexts)
				.Single(t => t.Id == fileId);

			Assert.StrictEqual(2, file.Contexts.Count);
		}

		[Fact]
		public void CanCreateFile()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			Assert.NotEqual(0, fileId);
			Assert.NotNull(fileManager.GetById(fileId));
		}

		[Fact]
		public void CanDeleteFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321", "order:123");

			Assert.Throws<InvalidOperationException>(() => fileManager.DeleteFile(fileId));
			fileManager.DeleteFile(fileId, true);
		}

		[Fact]
		public void CanDeleteFilesInBulk()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);
			var fileId2 = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId1, "invoice:1", "contract:1", "order:3");
			fileManager.AttachFileToContexts(fileId2, "invoice:2", "contract:2", "order:3");

			Assert.Throws<InvalidOperationException>(() => fileManager.DeleteFiles(new[] { fileId1, fileId2 }));

			fileManager.DeleteFiles(new[] { fileId1, fileId2 }, true);
		}

		[Fact]
		public void CanDetachFiles()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321", "order:123");

			fileManager.DetachFileFromContexts(fileId, "invoice:123", "order:123");

			var file = fileManager.FileContexts.Where(t => t.FileId == fileId).ToList();

			Assert.True(file.Count == 1);
			Assert.True(file[0].Value == "contract:321");
		}

		[Fact]
		public void CanGetFilesByContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId, "invoice:123", "contract:321");

			var files = fileManager.FileContexts.Where(t => t.Value == "invoice:123").ToList();
			Assert.Contains(files, t => t.FileId == fileId);
		}

		[Fact]
		public void CanRemoveContext()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId1 = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);
			var fileId2 = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			fileManager.AttachFileToContexts(fileId1, "invoice:1", "contract:1", "user:123");
			fileManager.AttachFileToContexts(fileId2, "invoice:2", "contract:2", "user:123");

			fileManager.DetachFilesFromContexts("user:123");

			var userFilesExist = fileManager.FileContexts.Any(t => t.Value == "user:123");
			Assert.False(userFilesExist);
		}

		[Fact]
		public void CanSpecifyUploader()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip, 12345);

			var file = fileManager.Files
				.SingleOrDefault(t => t.Id == fileId);

			Assert.StrictEqual(12345, file.CreatedByUserId);
		}
	}
}