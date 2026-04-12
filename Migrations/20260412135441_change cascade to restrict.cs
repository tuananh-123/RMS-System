using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class changecascadetorestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHistories_Recipes_RecipeID",
                table: "RecipeHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHistories_Recipes_RecipeID",
                table: "RecipeHistories",
                column: "RecipeID",
                principalTable: "Recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeHistories_Recipes_RecipeID",
                table: "RecipeHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeHistories_Recipes_RecipeID",
                table: "RecipeHistories",
                column: "RecipeID",
                principalTable: "Recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
