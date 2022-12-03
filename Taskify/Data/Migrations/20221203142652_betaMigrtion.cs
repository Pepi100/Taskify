using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    public partial class betaMigrtion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Tasks",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Tasks",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tasks",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Tasks",
                newName: "Date");
        }
    }
}
