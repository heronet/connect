using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace connect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUsernameToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12d28015-a635-4cc6-9e09-d165b84f6ad8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ba69887-4246-41bd-a88a-010c29352720");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5cf3fdca-8a3a-4a3b-bee5-1122288b4606", null, "Admin", "ADMIN" },
                    { "d428a14d-046a-4b79-858b-ff2460e376d6", null, "Member", "MEMBER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5cf3fdca-8a3a-4a3b-bee5-1122288b4606");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d428a14d-046a-4b79-858b-ff2460e376d6");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Comment");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12d28015-a635-4cc6-9e09-d165b84f6ad8", null, "Admin", "ADMIN" },
                    { "5ba69887-4246-41bd-a88a-010c29352720", null, "Member", "MEMBER" }
                });
        }
    }
}
