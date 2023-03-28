using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddUpdt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ratingsReview",
                columns: table => new
                {
                    RatingsReviewsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(nullable: false),
                    Reviews = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ProductDetailId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratingsReview", x => x.RatingsReviewsId);
                    table.ForeignKey(
                        name: "FK_ratingsReview_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ratingsReview_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ratingsReview_CustomerId",
                table: "ratingsReview",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ratingsReview_ProductDetailId",
                table: "ratingsReview",
                column: "ProductDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ratingsReview");
        }
    }
}
