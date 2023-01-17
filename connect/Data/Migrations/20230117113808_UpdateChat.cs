using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "646029e7-cef5-4084-8a10-8672de006c78");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "69865c5f-d07b-47ec-bbc3-4e010792ba2b");

            migrationBuilder.AddColumn<string>(
                name: "LastMessageSenderId",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "55031bae-a598-4561-bc79-51ee2c433385", null, "Member", "MEMBER" },
                    { "6a7b52f7-a332-44d6-a16f-41181d037e4b", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55031bae-a598-4561-bc79-51ee2c433385");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a7b52f7-a332-44d6-a16f-41181d037e4b");

            migrationBuilder.DropColumn(
                name: "LastMessageSenderId",
                table: "Chats");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "646029e7-cef5-4084-8a10-8672de006c78", null, "Admin", "ADMIN" },
                    { "69865c5f-d07b-47ec-bbc3-4e010792ba2b", null, "Member", "MEMBER" }
                });
        }
    }
}
