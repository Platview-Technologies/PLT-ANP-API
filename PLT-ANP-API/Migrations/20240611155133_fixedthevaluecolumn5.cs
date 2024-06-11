using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLT_ANP_API.Migrations
{
    public partial class fixedthevaluecolumn5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Value2",
                table: "Renewals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Value2",
                table: "Deals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value2",
                table: "Renewals");

            migrationBuilder.DropColumn(
                name: "Value2",
                table: "Deals");
        }
    }
}
