using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oty.Bot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    CommandType = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    HashCode = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => new { x.CommandType, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildState = table.Column<int>(type: "INTEGER", nullable: false),
                    ContainsBot = table.Column<bool>(type: "INTEGER", nullable: false),
                    RestrictionReason = table.Column<string>(type: "TEXT", nullable: true),
                    AllowAppeals = table.Column<bool>(type: "INTEGER", nullable: false),
                    LiftTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserState = table.Column<int>(type: "INTEGER", nullable: false),
                    BanReason = table.Column<string>(type: "TEXT", nullable: true),
                    BanLiftTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
