using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "varchar(100)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<string>(type: "varchar(max)", nullable: true),
                    Source = table.Column<string>(type: "varchar(500)", nullable: true),
                    Error = table.Column<string>(type: "varchar(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "varchar(100)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    StateType = table.Column<string>(type: "varchar(500)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logger = table.Column<string>(type: "nvarchar(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageRealTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_UsageRealTime", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_UsageStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthToken_AccountId",
                table: "AuthToken",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceHistory_DeviceId",
                table: "DeviceHistory",
                column: "DeviceId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceId",
                table: "Devices",
                column: "DeviceId",
                unique: true,
                filter: "[DeviceId] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_UsageRealTime_Date",
                table: "UsageRealTime",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthToken");

            migrationBuilder.DropTable(
                name: "DeviceHistory");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "UsageRealTime");

            migrationBuilder.DropTable(
                name: "UsageStats");
        }
    }
}
