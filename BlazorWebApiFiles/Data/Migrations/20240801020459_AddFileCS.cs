using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorIdentity.Files.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileCS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserNameUpdated",
                schema: "Files",
                table: "Folders",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UserNameInserted",
                schema: "Files",
                table: "Folders",
                newName: "InsertedBy");

            migrationBuilder.RenameColumn(
                name: "UserNameUpdated",
                schema: "Files",
                table: "Files",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "UserNameInserted",
                schema: "Files",
                table: "Files",
                newName: "InsertedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Files",
                table: "Folders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Files",
                table: "Folders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                schema: "Files",
                table: "Folders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Files",
                table: "Files",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Files",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                schema: "Files",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileTypeData",
                schema: "Files",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FileMapWithEntities",
                schema: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    InsertedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsertedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RequestedHashCode = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMapWithEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMapWithEntities",
                schema: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Files",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Files",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                schema: "Files",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                schema: "Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileTypeData",
                schema: "Files",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                schema: "Files",
                table: "Folders",
                newName: "UserNameUpdated");

            migrationBuilder.RenameColumn(
                name: "InsertedBy",
                schema: "Files",
                table: "Folders",
                newName: "UserNameInserted");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                schema: "Files",
                table: "Files",
                newName: "UserNameUpdated");

            migrationBuilder.RenameColumn(
                name: "InsertedBy",
                schema: "Files",
                table: "Files",
                newName: "UserNameInserted");
        }
    }
}
