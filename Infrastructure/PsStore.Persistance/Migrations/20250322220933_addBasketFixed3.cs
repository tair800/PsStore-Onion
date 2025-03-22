using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsStore.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addBasketFixed3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Baskets");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "BasketGames",
                newName: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BasketGames",
                newName: "Summary");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Baskets",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
