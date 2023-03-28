using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddTablesToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductInquiryStatus",
                table: "ProductInquiries",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ProductInquiries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProductInquiryRemark",
                columns: table => new
                {
                    ProductInquiryRemarksId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductInquiryId = table.Column<int>(nullable: false),
                    AdminRemarks = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInquiryRemark", x => x.ProductInquiryRemarksId);
                    table.ForeignKey(
                        name: "FK_ProductInquiryRemark_ProductInquiries_ProductInquiryId",
                        column: x => x.ProductInquiryId,
                        principalTable: "ProductInquiries",
                        principalColumn: "ProductInquiryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    PromotionsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotions", x => x.PromotionsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductInquiryRemark_ProductInquiryId",
                table: "ProductInquiryRemark",
                column: "ProductInquiryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductInquiryRemark");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductInquiries");

            migrationBuilder.AlterColumn<bool>(
                name: "ProductInquiryStatus",
                table: "ProductInquiries",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
