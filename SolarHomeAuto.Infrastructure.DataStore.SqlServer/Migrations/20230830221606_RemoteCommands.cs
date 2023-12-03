using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
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
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MonitoringServiceMode",
                table: "ApplicationState",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RemoteCommandMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", nullable: true),
                    RelatedId = table.Column<string>(type: "varchar(100)", nullable: true),
                    Data = table.Column<string>(type: "varchar(max)", nullable: true),
                    Source = table.Column<string>(type: "varchar(100)", nullable: true),
                    Consumed = table.Column<bool>(type: "bit", nullable: false),
                    ConsumedResult = table.Column<string>(type: "varchar(max)", nullable: true)
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
                column: "Type")
                .Annotation("SqlServer:Clustered", false);
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
