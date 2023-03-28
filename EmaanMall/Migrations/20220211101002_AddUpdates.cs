using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratingsReview_Customers_CustomerId",
                table: "ratingsReview");

            migrationBuilder.DropForeignKey(
                name: "FK_ratingsReview_ProductDetails_ProductDetailId",
                table: "ratingsReview");

            migrationBuilder.DropIndex(
                name: "IX_ratingsReview_CustomerId",
                table: "ratingsReview");

            migrationBuilder.DropIndex(
                name: "IX_ratingsReview_ProductDetailId",
                table: "ratingsReview");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ratingsReview");

            migrationBuilder.DropColumn(
                name: "ProductDetailId",
                table: "ratingsReview");

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "ratingsReview",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "isCustomerReviewed",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductColorsId",
                table: "OrderProducts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductSizesId",
                table: "OrderProducts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ratingsReview_OrderId",
                table: "ratingsReview",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ratingsReview_Orders_OrderId",
                table: "ratingsReview",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ratingsReview_Orders_OrderId",
                table: "ratingsReview");

            migrationBuilder.DropIndex(
                name: "IX_ratingsReview_OrderId",
                table: "ratingsReview");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ratingsReview");

            migrationBuilder.DropColumn(
                name: "isCustomerReviewed",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductColorsId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductSizesId",
                table: "OrderProducts");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ratingsReview",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductDetailId",
                table: "ratingsReview",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ratingsReview_CustomerId",
                table: "ratingsReview",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ratingsReview_ProductDetailId",
                table: "ratingsReview",
                column: "ProductDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ratingsReview_Customers_CustomerId",
                table: "ratingsReview",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ratingsReview_ProductDetails_ProductDetailId",
                table: "ratingsReview",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "ProductDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
