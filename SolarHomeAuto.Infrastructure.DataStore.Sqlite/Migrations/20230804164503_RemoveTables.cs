using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolarExcessJobs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolarExcessJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DelaySeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    JobId = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffGridPurchaseDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffGridPurchaseGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffProductionDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffProductionLessThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffTurnedOnAtLeastDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnBatteryMinPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnGridFeedInDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnGridFeedInGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnProductionDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnProductionGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnTurnedOffAtLeastDuration = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarExcessJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolarExcessJob_JobId",
                table: "SolarExcessJobs",
                column: "JobId",
                unique: true);
        }
    }
}
