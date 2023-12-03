using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class StateChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStateChange",
                table: "DeviceHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStateChange",
                table: "DeviceHistory");
        }
    }
}
