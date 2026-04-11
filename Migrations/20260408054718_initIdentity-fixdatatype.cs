using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RMS.Contants;

#nullable disable

namespace RMS.Migrations
{
    /// <inheritdoc />
    public partial class initIdentityfixdatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(BuildEnumTextToIntSql("Recipes", "Nation", typeof(Nation)));

            migrationBuilder.Sql(BuildEnumTextToIntSql("Recipes", "Difficulty", typeof(Difficulty)));

            migrationBuilder.Sql(BuildCuisineTextToIntArraySql("Recipes", "Cuisine"));

            migrationBuilder.AddColumn<int>(
                name: "LastedVersion",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(BuildEnumTextToIntSql("RecipeHistories", "Nation", typeof(Nation)));

            migrationBuilder.Sql(BuildEnumTextToIntSql("RecipeHistories", "Difficulty", typeof(Difficulty)));

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastedVersion",
                table: "Recipes");

            migrationBuilder.Sql(BuildEnumIntToTextSql("Recipes", "Nation", typeof(Nation)));

            migrationBuilder.Sql(BuildEnumIntToTextSql("Recipes", "Difficulty", typeof(Difficulty)));

            migrationBuilder.Sql(BuildCuisineIntArrayToTextSql("Recipes", "Cuisine"));

            migrationBuilder.Sql(BuildEnumIntToTextSql("RecipeHistories", "Nation", typeof(Nation)));

            migrationBuilder.Sql(BuildEnumIntToTextSql("RecipeHistories", "Difficulty", typeof(Difficulty)));
        }

        private static string BuildEnumTextToIntSql(string table, string column, Type enumType)
        {
            var cases = string.Join(
                Environment.NewLine,
                Enum.GetValues(enumType)
                    .Cast<object>()
                    .Select(value => $"        WHEN '{value}' THEN {Convert.ToInt32(value)}"));

            return
            $"ALTER TABLE \"{table}\"{Environment.NewLine}" +
            $"ALTER COLUMN \"{column}\" TYPE integer{Environment.NewLine}" +
            $"USING CASE \"{column}\"{Environment.NewLine}" +
            $"{cases}{Environment.NewLine}" +
            "        WHEN '' THEN -1" + Environment.NewLine +
            "        ELSE -1" + Environment.NewLine +
            "END;";
        }

        private static string BuildEnumIntToTextSql(string table, string column, Type enumType)
        {
            var cases = string.Join(
                Environment.NewLine,
                Enum.GetValues(enumType)
                    .Cast<object>()
                    .Select(value => $"        WHEN {Convert.ToInt32(value)} THEN '{value}'"));

            return
                $"ALTER TABLE \"{table}\"{Environment.NewLine}" +
                $"ALTER COLUMN \"{column}\" TYPE text{Environment.NewLine}" +
                $"USING CASE \"{column}\"{Environment.NewLine}" +
                $"{cases}{Environment.NewLine}" +
                "        ELSE 'Unknown'" + Environment.NewLine +
                "END;";
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
