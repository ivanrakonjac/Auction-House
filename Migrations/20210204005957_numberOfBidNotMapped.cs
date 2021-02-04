using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouse.Migrations
{
    public partial class numberOfBidNotMapped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "numberOfBids",
                table: "auctions");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "numberOfBids",
                table: "auctions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
