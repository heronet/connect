using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLastMessageTimeToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55031bae-a598-4561-bc79-51ee2c433385");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a7b52f7-a332-44d6-a16f-41181d037e4b");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageTime",
                table: "Chats",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12d28015-a635-4cc6-9e09-d165b84f6ad8", null, "Admin", "ADMIN" },
                    { "5ba69887-4246-41bd-a88a-010c29352720", null, "Member", "MEMBER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12d28015-a635-4cc6-9e09-d165b84f6ad8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ba69887-4246-41bd-a88a-010c29352720");

            migrationBuilder.DropColumn(
                name: "LastMessageTime",
                table: "Chats");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "55031bae-a598-4561-bc79-51ee2c433385", null, "Member", "MEMBER" },
                    { "6a7b52f7-a332-44d6-a16f-41181d037e4b", null, "Admin", "ADMIN" }
                });
        }
    }
}
