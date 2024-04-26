using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lesson55.Migrations
{
    /// <inheritdoc />
    public partial class AddedExecutorNameToMyTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExecutorName",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutorName",
                table: "Tasks");
        }
    }
}
