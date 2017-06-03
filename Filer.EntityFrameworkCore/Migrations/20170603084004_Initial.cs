namespace Filer.EntityFrameworkCore.Migrations
{
	using System;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class Initial : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "FileData");

			migrationBuilder.DropTable(
				name: "File");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "File",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					CompressionFormat = table.Column<byte>(nullable: false),
					CreatedOn = table.Column<DateTime>(nullable: false),
					Extension = table.Column<string>(maxLength: 20, nullable: true),
					MimeType = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
					Name = table.Column<string>(maxLength: 255, nullable: true),
					Size = table.Column<long>(nullable: false)
				},
				constraints: table => { table.PrimaryKey("PK_File", x => x.Id); });

			migrationBuilder.CreateTable(
				name: "FileData",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false),
					Data = table.Column<byte[]>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_FileData", x => x.Id);
					table.ForeignKey(
						name: "FK_FileData_File_Id",
						column: x => x.Id,
						principalTable: "File",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});
		}
	}
}