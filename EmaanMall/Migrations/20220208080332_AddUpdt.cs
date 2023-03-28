using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddUpdt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Vendor",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Vendor",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Vendor",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "productColor",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "productColor");
        }
    }
}
