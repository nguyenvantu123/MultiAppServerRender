using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class GenDBForDeviceFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                schema: "Identity",
                table: "DeviceFlowCodes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceFlowCodes",
                schema: "Identity",
                table: "DeviceFlowCodes",
                column: "DeviceCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceFlowCodes",
                schema: "Identity",
                table: "DeviceFlowCodes");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                schema: "Identity",
                table: "DeviceFlowCodes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
