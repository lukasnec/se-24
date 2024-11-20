using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace se_24.backend.Migrations
{
    /// <inheritdoc />
    public partial class ScoreAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerName",
                table: "Scores",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerName",
                table: "Scores");
        }
    }
}
