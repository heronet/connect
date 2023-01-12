using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class TextToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c52bc36a-4a81-4c00-892c-d6df45131719");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "def23b28-1404-4003-9da5-b38022533a51");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "48674317-1253-4747-817a-8e2a63d42c09", null, "Member", "MEMBER" },
                    { "d9e3a172-1754-44eb-87f7-35d08e3200e3", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48674317-1253-4747-817a-8e2a63d42c09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d9e3a172-1754-44eb-87f7-35d08e3200e3");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c52bc36a-4a81-4c00-892c-d6df45131719", null, "Member", "MEMBER" },
                    { "def23b28-1404-4003-9da5-b38022533a51", null, "Admin", "ADMIN" }
                });
        }
    }
}
