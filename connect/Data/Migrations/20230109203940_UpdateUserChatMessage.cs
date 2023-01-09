using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f0f1ff0-fd7a-44a1-908e-0e02a8747665");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d9529c2b-2fea-4ea6-9c71-c83c3683df96");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Message",
                newName: "SenderName");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<Dictionary<string, string>>(
                name: "Titles",
                table: "Chats",
                type: "hstore",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOnline",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1bfbb5cb-6335-4bed-a17b-b7218835a2f5", null, "Admin", "ADMIN" },
                    { "53a82a1b-da01-4159-abb3-d0239a76dd75", null, "Member", "MEMBER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bfbb5cb-6335-4bed-a17b-b7218835a2f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53a82a1b-da01-4159-abb3-d0239a76dd75");

            migrationBuilder.DropColumn(
                name: "Titles",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "LastOnline",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Message",
                newName: "UserName");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6f0f1ff0-fd7a-44a1-908e-0e02a8747665", null, "Member", "MEMBER" },
                    { "d9529c2b-2fea-4ea6-9c71-c83c3683df96", null, "Admin", "ADMIN" }
                });
        }
    }
}
