using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionHouse.Migrations
{
    public partial class tokensOrdersPriceIntToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<double>(
                name: "price",
                table: "tokensOrders",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AlterColumn<int>(
                name: "price",
                table: "tokensOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

           
        }
    }
}
