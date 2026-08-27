using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakCount2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreakCount",
                table: "DiaryEntries");

            migrationBuilder.AddColumn<int>(
                name: "StreakCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreakCount",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "StreakCount",
                table: "DiaryEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "DiaryEntries",
                keyColumn: "Id",
                keyValue: 1,
                column: "StreakCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "DiaryEntries",
                keyColumn: "Id",
                keyValue: 2,
                column: "StreakCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "DiaryEntries",
                keyColumn: "Id",
                keyValue: 3,
                column: "StreakCount",
                value: 0);
        }
    }
}
