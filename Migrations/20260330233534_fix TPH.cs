using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class fixTPH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Recipes_RecipeID",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_RecipeID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "RecipeHistories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Nation = table.Column<string>(type: "text", nullable: false),
                    Cuisine = table.Column<string>(type: "text", nullable: false),
                    Serving = table.Column<int>(type: "integer", nullable: false),
                    TotalCalories = table.Column<double>(type: "double precision", nullable: false),
                    CookingTime = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    ImageCover = table.Column<string>(type: "text", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    RecipeID = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    Trash = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecipeHistories_Recipes_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHistories_RecipeID",
                table: "RecipeHistories",
                column: "RecipeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeHistories");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Recipes",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RecipeID",
                table: "Recipes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "Recipes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeID",
                table: "Recipes",
                column: "RecipeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Recipes_RecipeID",
                table: "Recipes",
                column: "RecipeID",
                principalTable: "Recipes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
