using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BouvetBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamProfilePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamProfilePicture",
                table: "teams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamProfilePicture",
                table: "teams");
        }
    }
}
