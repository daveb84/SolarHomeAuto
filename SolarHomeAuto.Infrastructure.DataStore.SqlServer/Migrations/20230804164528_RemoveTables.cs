using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DelaySeconds = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    TurnOffGridPurchaseDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOffGridPurchaseGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOffProductionDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOffProductionLessThan = table.Column<int>(type: "int", nullable: false),
                    TurnOffTurnedOnAtLeastDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOnBatteryMinPercent = table.Column<int>(type: "int", nullable: false),
                    TurnOnGridFeedInDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOnGridFeedInGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOnProductionDuration = table.Column<int>(type: "int", nullable: false),
                    TurnOnProductionGreaterThan = table.Column<int>(type: "int", nullable: false),
                    TurnOnTurnedOffAtLeastDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarExcessJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolarExcessJob_JobId",
                table: "SolarExcessJobs",
                column: "JobId",
                unique: true,
                filter: "[JobId] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
