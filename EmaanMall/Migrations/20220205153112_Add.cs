using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "productColor",
                columns: table => new
                {
                    ProductColorsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductDetailId = table.Column<int>(nullable: false),
                    ColorName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productColor", x => x.ProductColorsId);
                    table.ForeignKey(
                        name: "FK_productColor_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productSize",
                columns: table => new
                {
                    ProductSizesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductDetailId = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: true),
                    TotalQuantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productSize", x => x.ProductSizesId);
                    table.ForeignKey(
                        name: "FK_productSize_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productColor_ProductDetailId",
                table: "productColor",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_productSize_ProductDetailId",
                table: "productSize",
                column: "ProductDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productColor");

            migrationBuilder.DropTable(
                name: "productSize");
        }
    }
}
