using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorWebApi.Users.Data.Migrations.TenantStoreDb
{
    /// <inheritdoc />
    public partial class GenerateApplicationTenantDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantInfo",
                schema: "Identity");

            migrationBuilder.CreateTable(
                name: "AppTenantInfo",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AppTenantInfo",
                columns: new[] { "Id", "ConnectionString", "Identifier", "Name" },
                values: new object[] { "Master", null, "Master", "Master" });

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantInfo_Identifier",
                schema: "Identity",
                table: "AppTenantInfo",
                column: "Identifier",
                unique: true,
                filter: "[Identifier] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTenantInfo",
                schema: "Identity");

            migrationBuilder.CreateTable(
                name: "TenantInfo",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "TenantInfo",
                columns: new[] { "Id", "ConnectionString", "Identifier", "Name" },
                values: new object[] { "Master", null, "Master", "Master" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantInfo_Identifier",
                schema: "Identity",
                table: "TenantInfo",
                column: "Identifier",
                unique: true,
                filter: "[Identifier] IS NOT NULL");
        }
    }
}
