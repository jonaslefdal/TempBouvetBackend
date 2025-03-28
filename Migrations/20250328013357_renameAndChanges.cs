using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BouvetBackend.Migrations
{
    /// <inheritdoc />
    public partial class renameAndChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userChallengeAttempts");

            migrationBuilder.DropColumn(
                name: "RequiredCount",
                table: "challenges");

            migrationBuilder.AlterColumn<int>(
                name: "MaxAttempts",
                table: "challenges",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ConditionType",
                table: "challenges",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RequiredDistanceKm",
                table: "challenges",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "userChallengeProgress",
                columns: table => new
                {
                    UserChallengeProgressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ChallengeId = table.Column<int>(type: "integer", nullable: false),
                    PointsAwarded = table.Column<int>(type: "integer", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userChallengeProgress", x => x.UserChallengeProgressId);
                    table.ForeignKey(
                        name: "FK_userChallengeProgress_challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "challenges",
                        principalColumn: "ChallengeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userChallengeProgress_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userChallengeProgress_ChallengeId",
                table: "userChallengeProgress",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_userChallengeProgress_UserId",
                table: "userChallengeProgress",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userChallengeProgress");

            migrationBuilder.DropColumn(
                name: "ConditionType",
                table: "challenges");

            migrationBuilder.DropColumn(
                name: "RequiredDistanceKm",
                table: "challenges");

            migrationBuilder.AlterColumn<int>(
                name: "MaxAttempts",
                table: "challenges",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredCount",
                table: "challenges",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "userChallengeAttempts",
                columns: table => new
                {
                    UserChallengeAttemptId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChallengeId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PointsAwarded = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userChallengeAttempts", x => x.UserChallengeAttemptId);
                    table.ForeignKey(
                        name: "FK_userChallengeAttempts_challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "challenges",
                        principalColumn: "ChallengeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userChallengeAttempts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userChallengeAttempts_ChallengeId",
                table: "userChallengeAttempts",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_userChallengeAttempts_UserId",
                table: "userChallengeAttempts",
                column: "UserId");
        }
    }
}
