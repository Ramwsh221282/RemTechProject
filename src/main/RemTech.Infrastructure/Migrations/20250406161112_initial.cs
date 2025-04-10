using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parsers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parser_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parser_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    next_run_unix_seconds = table.Column<long>(type: "bigint", nullable: false),
                    repeat_every_seconds = table.Column<long>(type: "bigint", nullable: false),
                    Links = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parser_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_parser_profiles_parsers_parser_id",
                        column: x => x.parser_id,
                        principalTable: "parsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_parser_profiles_name",
                table: "parser_profiles",
                column: "name",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_parser_profiles_parser_id",
                table: "parser_profiles",
                column: "parser_id");

            migrationBuilder.CreateIndex(
                name: "IX_parsers_name",
                table: "parsers",
                column: "name",
                unique: true,
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parser_profiles");

            migrationBuilder.DropTable(
                name: "parsers");
        }
    }
}
