using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class RenameAndAddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsageRealTime");

            migrationBuilder.DropTable(
                name: "UsageStats");

            migrationBuilder.CreateTable(
                name: "ApplicationState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    IsBackgroundServiceRunning = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarExcessJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobId = table.Column<string>(type: "TEXT", nullable: true),
                    DelaySeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    TurnOnBatteryMinPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnGridFeedInGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnGridFeedInDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnProductionGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnProductionDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOnTurnedOffAtLeastDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffGridPurchaseGreaterThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffGridPurchaseDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffProductionLessThan = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffProductionDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    TurnOffTurnedOnAtLeastDuration = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarExcessJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarRealTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Production = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridPower = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridPurchasing = table.Column<bool>(type: "INTEGER", nullable: false),
                    BatteryCharging = table.Column<bool>(type: "INTEGER", nullable: false),
                    BatteryPower = table.Column<decimal>(type: "TEXT", nullable: true),
                    Consumption = table.Column<decimal>(type: "TEXT", nullable: true),
                    BatteryCapacity = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarRealTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    Generation = table.Column<decimal>(type: "TEXT", nullable: true),
                    Consumption = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridFeedIn = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridPurchase = table.Column<decimal>(type: "TEXT", nullable: true),
                    ChargeCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    DischargeCapacity = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceHistory_Date",
                table: "DeviceHistory",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_SolarExcessJob_JobId",
                table: "SolarExcessJobs",
                column: "JobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarRealTime_Date",
                table: "SolarRealTime",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_SolarStats_Date",
                table: "SolarStats",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationState");

            migrationBuilder.DropTable(
                name: "SolarExcessJobs");

            migrationBuilder.DropTable(
                name: "SolarRealTime");

            migrationBuilder.DropTable(
                name: "SolarStats");

            migrationBuilder.DropIndex(
                name: "IX_DeviceHistory_Date",
                table: "DeviceHistory");

            migrationBuilder.CreateTable(
                name: "UsageRealTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BatteryCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    BatteryCharging = table.Column<bool>(type: "INTEGER", nullable: false),
                    BatteryPower = table.Column<decimal>(type: "TEXT", nullable: true),
                    Consumption = table.Column<decimal>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GridPower = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridPurchasing = table.Column<bool>(type: "INTEGER", nullable: false),
                    Production = table.Column<decimal>(type: "TEXT", nullable: true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageRealTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChargeCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    Consumption = table.Column<decimal>(type: "TEXT", nullable: true),
                    DischargeCapacity = table.Column<decimal>(type: "TEXT", nullable: true),
                    Duration = table.Column<string>(type: "TEXT", nullable: false),
                    Generation = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridFeedIn = table.Column<decimal>(type: "TEXT", nullable: true),
                    GridPurchase = table.Column<decimal>(type: "TEXT", nullable: true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsageRealTime_Date",
                table: "UsageRealTime",
                column: "Date");
        }
    }
}
