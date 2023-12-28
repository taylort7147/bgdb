using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BggExt.Migrations
{
    /// <inheritdoc />
    public partial class BoardGameLibraryDataSyncFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronizationEnabled",
                table: "Libraries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronizationEnabled",
                table: "Libraries");
        }
    }
}
