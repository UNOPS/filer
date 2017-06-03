namespace Filer.EntityFrameworkCore.Tests
{
	using Filer.Core;
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
		public void CanCreateFile()
		{
			var fileManager = new FileManager(this.dbFixture.CreateDataContext());
			var file = fileManager.SaveFile("test.txt", "text/plain", new byte[0], CompressionFormat.GZip, "ef-test");

			Assert.NotEqual(0, file.Id);
			Assert.NotNull(fileManager.GetById(file.Id));
		}
	}
}