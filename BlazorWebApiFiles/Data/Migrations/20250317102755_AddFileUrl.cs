using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Files.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                schema: "Files",
                table: "Files",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Files",
                table: "Files",
                keyColumn: "FileUrl",
                keyValue: null,
                column: "FileUrl",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                schema: "Files",
                table: "Files",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
