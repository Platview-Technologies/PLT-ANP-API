using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLT_ANP_API.Migrations
{
    public partial class Triedadiffstructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_AspNetUsers_UserId",
                table: "EmailLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "EmailLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserModelId",
                table: "EmailLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_UserModelId",
                table: "EmailLogs",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_AspNetUsers_UserModelId",
                table: "EmailLogs",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_TempUser_UserId",
                table: "EmailLogs",
                column: "UserId",
                principalTable: "TempUser",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_AspNetUsers_UserModelId",
                table: "EmailLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_TempUser_UserId",
                table: "EmailLogs");

            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_UserModelId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "EmailLogs");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EmailLogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_AspNetUsers_UserId",
                table: "EmailLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
