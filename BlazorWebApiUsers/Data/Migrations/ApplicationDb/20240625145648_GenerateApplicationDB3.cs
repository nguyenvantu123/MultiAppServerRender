using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorWebApi.Users.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class GenerateApplicationDB3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserTokens",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserRoles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserLogins",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserClaims",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetRoles",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetRoleClaims",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Identity",
                table: "AspNetRoleClaims");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "Identity",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
