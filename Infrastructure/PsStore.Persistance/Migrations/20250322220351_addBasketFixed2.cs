using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsStore.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addBasketFixed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BasketGames",
                newName: "Summary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "BasketGames",
                newName: "Price");
        }
    }
}
