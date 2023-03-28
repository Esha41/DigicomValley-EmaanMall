using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddPassToVendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Vendor",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Vendor");
        }
    }
}
