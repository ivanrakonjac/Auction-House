using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouse.Migrations
{
    public partial class AddUserGender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "gender",
                table: "AspNetUsers");

        }
    }
}
