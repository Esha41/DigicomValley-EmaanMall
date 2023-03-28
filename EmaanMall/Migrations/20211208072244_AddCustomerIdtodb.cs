using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddCustomerIdtodb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ProductInquiries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductInquiries_CustomerId",
                table: "ProductInquiries",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInquiries_Customers_CustomerId",
                table: "ProductInquiries",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInquiries_Customers_CustomerId",
                table: "ProductInquiries");

            migrationBuilder.DropIndex(
                name: "IX_ProductInquiries_CustomerId",
                table: "ProductInquiries");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductInquiries");
        }
    }
}
