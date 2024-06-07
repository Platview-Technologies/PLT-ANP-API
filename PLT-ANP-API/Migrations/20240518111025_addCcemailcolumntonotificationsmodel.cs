using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLT_ANP_API.Migrations
{
    public partial class addCcemailcolumntonotificationsmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CCEmails",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CCEmails",
                table: "Notifications");
        }
    }
}
