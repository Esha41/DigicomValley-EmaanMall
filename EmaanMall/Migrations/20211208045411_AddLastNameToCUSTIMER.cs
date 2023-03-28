using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddLastNameToCUSTIMER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
