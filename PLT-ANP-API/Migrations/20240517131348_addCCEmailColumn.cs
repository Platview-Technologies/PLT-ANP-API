using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLT_ANP_API.Migrations
{
    public partial class addCCEmailColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "CCEmails",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCEmails",
                table: "Deals");

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Deals",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
