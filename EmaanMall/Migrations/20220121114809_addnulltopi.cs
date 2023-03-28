using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class addnulltopi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductDetails_ProductDetailId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductDetailId",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductDetails_ProductDetailId",
                table: "ProductImages",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "ProductDetailId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductDetails_ProductDetailId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<int>(
                name: "ProductDetailId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductDetails_ProductDetailId",
                table: "ProductImages",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "ProductDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
