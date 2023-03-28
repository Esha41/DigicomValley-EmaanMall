using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmaanMall.Migrations
{
    public partial class AddTablesToDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "promoCodes",
                columns: table => new
                {
                    PromoCodesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    discountPercentage = table.Column<double>(nullable: false),
                    NoOfUsage = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promoCodes", x => x.PromoCodesId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promoCodes");
        }
    }
}
