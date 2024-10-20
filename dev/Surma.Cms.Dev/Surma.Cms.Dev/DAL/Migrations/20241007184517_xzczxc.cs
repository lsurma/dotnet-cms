using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surma.Cms.Dev.DAL.Migrations
{
    /// <inheritdoc />
    public partial class xzczxc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentMimeType",
                table: "AssetRevisions",
                newName: "ContentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "AssetRevisions",
                newName: "ContentMimeType");
        }
    }
}
