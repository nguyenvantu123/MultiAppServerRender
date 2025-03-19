using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class ThemCaiAvatar1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "Identity",
                table: "UserProfiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Identity",
                table: "UserProfiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "Identity",
                table: "UserProfiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "Identity",
                table: "UserProfiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "Identity",
                table: "UserProfiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "Identity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Identity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "Identity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "Identity",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "Identity",
                table: "UserProfiles");
        }
    }
}
