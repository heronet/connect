using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class Photo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "408ac361-fd51-4a06-af9d-1da87f2d13a8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b23bc01c-b5ed-436b-b330-844fd944db2f");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Photo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Photo",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "646029e7-cef5-4084-8a10-8672de006c78", null, "Admin", "ADMIN" },
                    { "69865c5f-d07b-47ec-bbc3-4e010792ba2b", null, "Member", "MEMBER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "646029e7-cef5-4084-8a10-8672de006c78");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "69865c5f-d07b-47ec-bbc3-4e010792ba2b");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Photo");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "408ac361-fd51-4a06-af9d-1da87f2d13a8", null, "Member", "MEMBER" },
                    { "b23bc01c-b5ed-436b-b330-844fd944db2f", null, "Admin", "ADMIN" }
                });
        }
    }
}
