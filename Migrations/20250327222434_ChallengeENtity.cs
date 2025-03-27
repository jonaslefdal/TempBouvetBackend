using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BouvetBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChallengeENtity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredCount",
                table: "challenges",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredTransportMethod",
                table: "challenges",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredCount",
                table: "challenges");

            migrationBuilder.DropColumn(
                name: "RequiredTransportMethod",
                table: "challenges");
        }
    }
}
