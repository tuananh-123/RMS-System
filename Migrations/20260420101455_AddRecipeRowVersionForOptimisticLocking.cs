using Microsoft.EntityFrameworkCore.Migrations;
using RMS.Entities;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeRowVersionForOptimisticLocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchKeyword",
                table: "Recipes",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(SearchKeyword),
                oldType: "jsonb");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Recipes",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SearchKeyword",
                table: "Ingredients",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(SearchKeyword),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Recipes");

            migrationBuilder.AlterColumn<SearchKeyword>(
                name: "SearchKeyword",
                table: "Recipes",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<SearchKeyword>(
                name: "SearchKeyword",
                table: "Ingredients",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
