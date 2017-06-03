namespace Filer.EntityFrameworkCore.Migrations
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class FileOwner : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_File_Owner",
				table: "File");

			migrationBuilder.DropColumn(
				name: "Owner",
				table: "File");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Owner",
				table: "File",
				maxLength: 50,
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_File_Owner",
				table: "File",
				column: "Owner");
		}
	}
}