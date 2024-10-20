using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surma.Cms.Dev.DAL.Migrations
{
    /// <inheritdoc />
    public partial class xx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Assets_ContentAssetId",
                table: "Pages");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Assets",
                newName: "DisplayName");

            migrationBuilder.AddColumn<bool>(
                name: "IsStatic",
                table: "Assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Assets_ContentAssetId",
                table: "Pages",
                column: "ContentAssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Assets_ContentAssetId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "IsStatic",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Assets",
                newName: "Slug");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Assets_ContentAssetId",
                table: "Pages",
                column: "ContentAssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
