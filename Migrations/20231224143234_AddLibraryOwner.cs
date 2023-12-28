using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BggExt.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Libraries_LibraryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LibraryId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LibraryId",
                table: "AspNetUsers",
                newName: "Libraries");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Libraries",
                table: "AspNetUsers",
                column: "Libraries",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Libraries_Libraries",
                table: "AspNetUsers",
                column: "Libraries",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Libraries_Libraries",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Libraries",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Libraries",
                table: "AspNetUsers",
                newName: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LibraryId",
                table: "AspNetUsers",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Libraries_LibraryId",
                table: "AspNetUsers",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
