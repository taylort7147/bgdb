using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BggExt.Migrations
{
    /// <inheritdoc />
    public partial class RenameLibraryIsSynchronizationEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSynchronizationEnabled",
                table: "Libraries",
                newName: "IsEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "Libraries",
                newName: "IsSynchronizationEnabled");
        }
    }
}
