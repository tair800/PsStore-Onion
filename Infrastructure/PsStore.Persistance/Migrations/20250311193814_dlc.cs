using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsStore.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class dlc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dlc");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Dlc",
                newName: "ImgUrl");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Dlc",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Dlc",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Dlc");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Dlc");

            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Dlc",
                newName: "Image");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dlc",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
