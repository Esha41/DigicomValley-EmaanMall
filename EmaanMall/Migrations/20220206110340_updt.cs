using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class updt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorBusiness_ProductCategories_CategoryId",
                table: "VendorBusiness");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBusiness_Categories_CategoryId",
                table: "VendorBusiness",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorBusiness_Categories_CategoryId",
                table: "VendorBusiness");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorBusiness_ProductCategories_CategoryId",
                table: "VendorBusiness",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
