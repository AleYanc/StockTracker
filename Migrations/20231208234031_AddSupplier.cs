using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 8, 20, 40, 30, 726, DateTimeKind.Local).AddTicks(4708),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 6, 16, 25, 5, 86, DateTimeKind.Local).AddTicks(8914));

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Supplier_SupplierId",
                table: "Products",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Supplier_SupplierId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Products");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 12, 6, 16, 25, 5, 86, DateTimeKind.Local).AddTicks(8914),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 12, 8, 20, 40, 30, 726, DateTimeKind.Local).AddTicks(4708));
        }
    }
}
