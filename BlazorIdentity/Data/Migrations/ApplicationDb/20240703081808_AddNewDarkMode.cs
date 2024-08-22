using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Users.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddNewDarkMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDarkMode",
                schema: "Identity",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDarkMode",
                schema: "Identity",
                table: "UserProfiles");
        }
    }
}
