using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLT_ANP_API.Migrations
{
    public partial class addedColumnRenewalDate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RenewalDate",
                table: "Deals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalDate",
                table: "Deals");
        }
    }
}
