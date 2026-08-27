using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class dbfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_DiaryEntryId",
                table: "Reactions",
                column: "DiaryEntryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_DiaryEntries_DiaryEntryId",
                table: "Reactions",
                column: "DiaryEntryId",
                principalTable: "DiaryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_DiaryEntries_DiaryEntryId",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_DiaryEntryId",
                table: "Reactions");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Reactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
