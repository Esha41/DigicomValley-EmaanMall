using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddDsPr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountPrice",
                table: "ProductBundles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "ProductBundles");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Customers");
        }
    }
}
