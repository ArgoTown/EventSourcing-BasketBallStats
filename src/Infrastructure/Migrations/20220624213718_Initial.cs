using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BasketBallStats.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Arenas",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                ArenaId = table.Column<Guid>(type: "Uuid", nullable: false),
                Name = table.Column<string>(type: "Varchar", maxLength: 255, nullable: false),
                Capacity = table.Column<int>(type: "Integer", nullable: false),
                Address = table.Column<string>(type: "Varchar", maxLength: 255, nullable: false),
                Built = table.Column<DateOnly>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Arenas", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                GameId = table.Column<Guid>(type: "Uuid", nullable: false),
                TeamHomeId = table.Column<Guid>(type: "Uuid", nullable: false),
                TeamAwayId = table.Column<Guid>(type: "Uuid", nullable: false),
                GameTime = table.Column<DateTime>(type: "TimestampTz", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ReadModelStatistics",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                GameId = table.Column<Guid>(type: "Uuid", nullable: false),
                TeamId = table.Column<Guid>(type: "Uuid", nullable: false),
                PlayerId = table.Column<Guid>(type: "Uuid", nullable: false),
                MadeFreeThrows = table.Column<short>(type: "smallint", nullable: false),
                MissedFreeThrows = table.Column<short>(type: "smallint", nullable: false),
                MadeTwoPoints = table.Column<short>(type: "smallint", nullable: false),
                MissedTwoPoints = table.Column<short>(type: "smallint", nullable: false),
                MadeThreePoints = table.Column<short>(type: "smallint", nullable: false),
                MissedThreePoints = table.Column<short>(type: "smallint", nullable: false),
                DefensiveRebounds = table.Column<short>(type: "smallint", nullable: false),
                OffensiveRebounds = table.Column<short>(type: "smallint", nullable: false),
                Steals = table.Column<short>(type: "smallint", nullable: false),
                Turnovers = table.Column<short>(type: "smallint", nullable: false),
                Blocks = table.Column<short>(type: "smallint", nullable: false),
                BlocksReceived = table.Column<short>(type: "smallint", nullable: false),
                Fouls = table.Column<short>(type: "smallint", nullable: false),
                FoulsProvoked = table.Column<short>(type: "smallint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReadModelStatistics", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Streams",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                StreamId = table.Column<Guid>(type: "Uuid", nullable: false),
                EventId = table.Column<Guid>(type: "Uuid", nullable: false),
                Type = table.Column<string>(type: "Varchar", maxLength: 255, nullable: false),
                Data = table.Column<string>(type: "Text", nullable: false),
                MetaData = table.Column<string>(type: "Varchar", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Streams", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Teams",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                TeamId = table.Column<Guid>(type: "Uuid", nullable: false),
                Name = table.Column<string>(type: "Varchar", maxLength: 255, nullable: false),
                Founded = table.Column<DateOnly>(type: "date", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                GameId = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Teams", x => x.Id);
                table.ForeignKey(
                    name: "FK_Teams_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ArenaTeam",
            columns: table => new
            {
                ArenasId = table.Column<long>(type: "bigint", nullable: false),
                TeamsId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArenaTeam", x => new { x.ArenasId, x.TeamsId });
                table.ForeignKey(
                    name: "FK_ArenaTeam_Arenas_ArenasId",
                    column: x => x.ArenasId,
                    principalTable: "Arenas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ArenaTeam_Teams_TeamsId",
                    column: x => x.TeamsId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Players",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                MiddleName = table.Column<string>(type: "text", nullable: true),
                Surname = table.Column<string>(type: "text", nullable: false),
                BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                Nationality = table.Column<string>(type: "text", nullable: false),
                Weight = table.Column<short>(type: "smallint", nullable: false),
                Height = table.Column<short>(type: "smallint", nullable: false),
                TeamId = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Players", x => x.Id);
                table.ForeignKey(
                    name: "FK_Players_Teams_TeamId",
                    column: x => x.TeamId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ArenaTeam_TeamsId",
            table: "ArenaTeam",
            column: "TeamsId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_GameId",
            table: "Games",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_Players_TeamId",
            table: "Players",
            column: "TeamId");

        migrationBuilder.CreateIndex(
            name: "IX_Streams_EventId",
            table: "Streams",
            column: "EventId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Streams_StreamId",
            table: "Streams",
            column: "StreamId");

        migrationBuilder.CreateIndex(
            name: "IX_Teams_GameId",
            table: "Teams",
            column: "GameId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ArenaTeam");

        migrationBuilder.DropTable(
            name: "Players");

        migrationBuilder.DropTable(
            name: "ReadModelStatistics");

        migrationBuilder.DropTable(
            name: "Streams");

        migrationBuilder.DropTable(
            name: "Arenas");

        migrationBuilder.DropTable(
            name: "Teams");

        migrationBuilder.DropTable(
            name: "Games");
    }
}
