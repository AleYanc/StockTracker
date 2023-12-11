using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddExcelImportHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 11, 13, 25, 42, 332, DateTimeKind.Local).AddTicks(3386),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 8, 20, 40, 30, 726, DateTimeKind.Local).AddTicks(4708));

            migrationBuilder.CreateTable(
                name: "ImportHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 12, 11, 13, 25, 42, 332, DateTimeKind.Local).AddTicks(3869))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportHistories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 8, 20, 40, 30, 726, DateTimeKind.Local).AddTicks(4708),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 11, 13, 25, 42, 332, DateTimeKind.Local).AddTicks(3386));
        }
    }
}
