using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class indexandUniqueTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_Title",
                table: "Tags",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Title",
                table: "Recipes",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Title",
                table: "Ingredients",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Title",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_Title",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_Title",
                table: "Ingredients");
        }
    }
}
