namespace Filer.EntityFrameworkCore.Tests
{
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

			var file = await fileManager.GetAll()
				.Include(t => t.Contexts)
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(2, file.Contexts.Count);
		}

		[Fact]
		public async void CanSpecifyUploader()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip, 12345);

			var file = await fileManager.GetAll()
				.SingleOrDefaultAsync(t => t.Id == fileId);

			Assert.StrictEqual(12345, file.CreatedByUserId);
		}

		[Fact]
		public async void CanCreateFile()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var fileId = await fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip);

			Assert.NotEqual(0, fileId);
			Assert.NotNull(fileManager.GetById(fileId));
		}
	}
}