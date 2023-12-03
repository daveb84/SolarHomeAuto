using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class RemoteCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMonitoringWorkerRunning",
                table: "ApplicationState",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MonitoringServiceMode",
                table: "ApplicationState",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RemoteCommandMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    RelatedId = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    Consumed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConsumedResult = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteCommandMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoteCommandMessages_MessageId",
                table: "RemoteCommandMessages",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemoteCommandMessages_Type",
                table: "RemoteCommandMessages",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoteCommandMessages");

            migrationBuilder.DropColumn(
                name: "IsMonitoringWorkerRunning",
                table: "ApplicationState");

            migrationBuilder.DropColumn(
                name: "MonitoringServiceMode",
                table: "ApplicationState");
        }
    }
}
