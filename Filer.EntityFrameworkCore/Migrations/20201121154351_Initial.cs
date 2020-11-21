namespace Filer.EntityFrameworkCore.Migrations
{
	using System;
	using Microsoft.EntityFrameworkCore.Migrations;

	public partial class Initial : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "FileContext");

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
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CompressionFormat = table.Column<byte>(type: "tinyint", nullable: false),
					CreatedByUserId = table.Column<int>(type: "int", nullable: true),
					CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
					Extension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
					MimeType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
					Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
					Size = table.Column<long>(type: "bigint", nullable: false)
				},
				constraints: table => { table.PrimaryKey("PK_File", x => x.Id); });

			migrationBuilder.CreateTable(
				name: "FileContext",
				columns: table => new
				{
					FileId = table.Column<int>(type: "int", nullable: false),
					Value = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_FileContext", x => new { x.FileId, x.Value });
					table.ForeignKey(
						name: "FK_FileContext_File_FileId",
						column: x => x.FileId,
						principalTable: "File",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "FileData",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false),
					Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
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

			migrationBuilder.CreateIndex(
				name: "IX_File_CreatedByUserId",
				table: "File",
				column: "CreatedByUserId");

			migrationBuilder.CreateIndex(
				name: "IX_FileContext_Value_FileId",
				table: "FileContext",
				columns: new[] { "Value", "FileId" },
				unique: true);
		}
	}
}