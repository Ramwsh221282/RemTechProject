using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TransportCharacteristicsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "characteristics",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "characteristics",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "characteristics",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "characteristics",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "characteristics",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "characteristics",
                newName: "Id");
        }
    }
}
