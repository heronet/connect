using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bfbb5cb-6335-4bed-a17b-b7218835a2f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53a82a1b-da01-4159-abb3-d0239a76dd75");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "Message",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "30800b2b-e572-4c80-b1c2-ba12882736ed", null, "Admin", "ADMIN" },
                    { "80ed0a70-ef64-4776-87ce-d7a30dd4a640", null, "Member", "MEMBER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30800b2b-e572-4c80-b1c2-ba12882736ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80ed0a70-ef64-4776-87ce-d7a30dd4a640");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Message");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1bfbb5cb-6335-4bed-a17b-b7218835a2f5", null, "Admin", "ADMIN" },
                    { "53a82a1b-da01-4159-abb3-d0239a76dd75", null, "Member", "MEMBER" }
                });
        }
    }
}
