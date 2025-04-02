using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BouvetBackend.Migrations
{
    /// <inheritdoc />
    public partial class EnumFixForMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "transportEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "RequiredTransportMethod",
                table: "challenges");

            migrationBuilder.AddColumn<int>(
                name: "RequiredTransportMethod",
                table: "challenges",
                type: "integer",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "transportEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RequiredTransportMethod",
                table: "challenges",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
