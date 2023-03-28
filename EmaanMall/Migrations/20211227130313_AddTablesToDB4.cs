using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddTablesToDB4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discountPercentage",
                table: "promoCodes");

            migrationBuilder.AlterColumn<long>(
                name: "promoCode",
                table: "promoCodes",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "discountPrice",
                table: "promoCodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerPromoCodeLogs",
                columns: table => new
                {
                    CustomerPromoCodeLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoCodesId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPromoCodeLogs", x => x.CustomerPromoCodeLogId);
                    table.ForeignKey(
                        name: "FK_CustomerPromoCodeLogs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPromoCodeLogs_promoCodes_PromoCodesId",
                        column: x => x.PromoCodesId,
                        principalTable: "promoCodes",
                        principalColumn: "PromoCodesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPromoCodeLogs_CustomerId",
                table: "CustomerPromoCodeLogs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPromoCodeLogs_PromoCodesId",
                table: "CustomerPromoCodeLogs",
                column: "PromoCodesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPromoCodeLogs");

            migrationBuilder.DropColumn(
                name: "discountPrice",
                table: "promoCodes");

            migrationBuilder.AlterColumn<string>(
                name: "promoCode",
                table: "promoCodes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "discountPercentage",
                table: "promoCodes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
