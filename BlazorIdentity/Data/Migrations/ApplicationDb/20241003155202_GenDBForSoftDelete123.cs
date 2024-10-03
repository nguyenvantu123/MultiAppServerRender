using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class GenDBForSoftDelete123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
