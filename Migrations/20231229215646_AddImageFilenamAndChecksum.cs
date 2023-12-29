using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BggExt.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFilenamAndChecksum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Checksum",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checksum",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "Images");
        }
    }
}
