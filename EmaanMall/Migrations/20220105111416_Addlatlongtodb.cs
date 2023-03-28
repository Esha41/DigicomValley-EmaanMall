using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class Addlatlongtodb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Customers");
        }
    }
}
