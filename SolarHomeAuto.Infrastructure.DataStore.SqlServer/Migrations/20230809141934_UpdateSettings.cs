using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthToken_AccountId",
                table: "AuthToken");

            migrationBuilder.DropColumn(
                name: "AuthSettings",
                table: "ApplicationState");

            migrationBuilder.RenameColumn(
                name: "SolarSettings",
                table: "ApplicationState",
                newName: "ServerApiAccount");

            migrationBuilder.RenameColumn(
                name: "ShellySettings",
                table: "ApplicationState",
                newName: "AccountCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "AuthToken",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "AuthToken",
                type: "varchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AuthToken_ServiceId",
                table: "AuthToken",
                column: "ServiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthToken_ServiceId",
                table: "AuthToken");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "AuthToken");

            migrationBuilder.RenameColumn(
                name: "ServerApiAccount",
                table: "ApplicationState",
                newName: "SolarSettings");

            migrationBuilder.RenameColumn(
                name: "AccountCredentials",
                table: "ApplicationState",
                newName: "ShellySettings");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "AuthToken",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthSettings",
                table: "ApplicationState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthToken_AccountId",
                table: "AuthToken",
                column: "AccountId",
                unique: true);
        }
    }
}
