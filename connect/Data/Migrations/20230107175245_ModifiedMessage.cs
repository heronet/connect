using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68d60111-6adc-4dd8-84b3-c47524fb6a62");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc3a14a5-fb31-4055-8c03-23a5c0178a41");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Message",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessageSender",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6f0f1ff0-fd7a-44a1-908e-0e02a8747665", null, "Member", "MEMBER" },
                    { "d9529c2b-2fea-4ea6-9c71-c83c3683df96", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f0f1ff0-fd7a-44a1-908e-0e02a8747665");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d9529c2b-2fea-4ea6-9c71-c83c3683df96");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "LastMessageSender",
                table: "Chats");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "68d60111-6adc-4dd8-84b3-c47524fb6a62", null, "Member", "MEMBER" },
                    { "bc3a14a5-fb31-4055-8c03-23a5c0178a41", null, "Admin", "ADMIN" }
                });
        }
    }
}
