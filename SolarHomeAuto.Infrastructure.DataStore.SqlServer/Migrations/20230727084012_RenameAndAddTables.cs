using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsBackgroundServiceRunning = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarExcessJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DelaySeconds = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TurnOnBatteryMinPercent = table.Column<int>(type: "int", nullable: false),
                    TurnOnGridFeedInGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOnGridFeedInDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOnProductionGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOnProductionDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOnTurnedOffAtLeastDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOffGridPurchaseGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOffGridPurchaseDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOffProductionLessThan = table.Column<int>(type: "int", nullable: false),
                    TurnOffProductionDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOffTurnedOnAtLeastDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarExcessJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarRealTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Production = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridPower = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridPurchasing = table.Column<bool>(type: "bit", nullable: false),
                    BatteryCharging = table.Column<bool>(type: "bit", nullable: false),
                    BatteryPower = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Consumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatteryCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarRealTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Generation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Consumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridFeedIn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChargeCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DischargeCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceHistory_Date",
                table: "DeviceHistory",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_SolarExcessJob_JobId",
                table: "SolarExcessJobs",
                column: "JobId",
                unique: true,
                filter: "[JobId] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_SolarRealTime_Date",
                table: "SolarRealTime",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_SolarStats_Date",
                table: "SolarStats",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatteryCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BatteryCharging = table.Column<bool>(type: "bit", nullable: false),
                    BatteryPower = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Consumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    GridPower = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridPurchasing = table.Column<bool>(type: "bit", nullable: false),
                    Production = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageRealTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargeCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Consumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DischargeCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Generation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridFeedIn = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GridPurchase = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsageRealTime_Date",
                table: "UsageRealTime",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
