using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouse.Migrations
{
    public partial class RolesInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8f35eca0-ce4a-45d5-b889-9f21aa07c83b", "977f6fb0-7c26-4633-aa34-e89289790f50", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d0f310ad-e0cd-416e-be60-5923f95ffd20", "e1ef8afe-3fbb-4e6a-ab22-6cb82c2aaaf2", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f35eca0-ce4a-45d5-b889-9f21aa07c83b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0f310ad-e0cd-416e-be60-5923f95ffd20");
        }
    }
}
