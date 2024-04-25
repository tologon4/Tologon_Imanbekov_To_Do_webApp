using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lesson55.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateOfClosingToMyTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfClosing",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfClosing",
                table: "Tasks");
        }
    }
}
