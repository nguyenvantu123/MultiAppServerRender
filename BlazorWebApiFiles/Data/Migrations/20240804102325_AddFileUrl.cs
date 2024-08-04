using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorWebApi.Files.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                schema: "Files",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                schema: "Files",
                table: "Files");
        }
    }
}
