using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class tictactoe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicTacToes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player1Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player2Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentTurnPlayerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WinnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGameOver = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicTacToes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicTacToes");
        }
    }
}
