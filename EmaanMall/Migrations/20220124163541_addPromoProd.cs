using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class addPromoProd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionsProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionsId = table.Column<int>(nullable: false),
                    ProductDetailId = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionsProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionsProduct_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionsProduct_promotions_PromotionsId",
                        column: x => x.PromotionsId,
                        principalTable: "promotions",
                        principalColumn: "PromotionsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsProduct_ProductDetailId",
                table: "PromotionsProduct",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionsProduct_PromotionsId",
                table: "PromotionsProduct",
                column: "PromotionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionsProduct");
        }
    }
}
