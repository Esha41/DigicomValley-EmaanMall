using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddStatustodb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.AlterColumn<bool>(
                name: "OrderStatus",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
