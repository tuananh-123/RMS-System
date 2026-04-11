using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class alignRecipeHistoryCuisine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(BuildCuisineTextToIntArraySql("RecipeHistories", "Cuisine"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(BuildCuisineIntArrayToTextSql("RecipeHistories", "Cuisine"));
        }

        private static string BuildCuisineTextToIntArraySql(string table, string column)
        {
            return
                $"ALTER TABLE \"{table}\"{Environment.NewLine}" +
                $"ALTER COLUMN \"{column}\" TYPE integer[]{Environment.NewLine}" +
                "USING CASE" + Environment.NewLine +
                $"        WHEN \"{column}\" IS NULL OR btrim(\"{column}\") = '' THEN ARRAY[]::integer[]{Environment.NewLine}" +
                $"        WHEN \"{column}\" ~ '^\\{{[-0-9, ]*\\}}$' THEN \"{column}\"::integer[]{Environment.NewLine}" +
                $"        WHEN \"{column}\" ~ '^\\[[-0-9, ]*\\]$' THEN replace(replace(\"{column}\", '[', '{{'), ']', '}}')::integer[]{Environment.NewLine}" +
                $"        WHEN \"{column}\" ~ '^-?[0-9]+(\\s*,\\s*-?[0-9]+)*$' THEN string_to_array(regexp_replace(\"{column}\", '\\s+', '', 'g'), ',')::integer[]{Environment.NewLine}" +
                "        ELSE ARRAY[]::integer[]" + Environment.NewLine +
                "END;";
        }

        private static string BuildCuisineIntArrayToTextSql(string table, string column)
        {
            return
                $"ALTER TABLE \"{table}\"{Environment.NewLine}" +
                $"ALTER COLUMN \"{column}\" TYPE text{Environment.NewLine}" +
                $"USING array_to_string(\"{column}\", ',');";
        }
    }
}
