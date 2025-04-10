using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisementsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "advertisements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    price_extra = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    price_value = table.Column<long>(type: "bigint", nullable: false),
                    published_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scraper_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_url = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    characteristics = table.Column<string>(type: "jsonb", nullable: false),
                    photos = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advertisements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characteristics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characteristics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advertisements");

            migrationBuilder.DropTable(
                name: "characteristics");
        }
    }
}
