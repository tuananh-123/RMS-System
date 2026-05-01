using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class addruleforslugcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "RecipeHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Ingredients",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Slug",
                table: "Recipes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Slug",
                table: "Ingredients",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Slug",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_Slug",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_Slug",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "RecipeHistories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Ingredients");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
