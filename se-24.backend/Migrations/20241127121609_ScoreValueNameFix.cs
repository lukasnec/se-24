using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace se_24.backend.Migrations
{
    /// <inheritdoc />
    public partial class ScoreValueNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "Scores",
                newName: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Scores",
                newName: "value");
        }
    }
}
