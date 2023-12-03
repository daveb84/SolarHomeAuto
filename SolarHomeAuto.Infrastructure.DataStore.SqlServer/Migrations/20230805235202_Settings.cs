﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarHomeAuto.Infrastructure.DataStore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthSettings",
                table: "ApplicationState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShellySettings",
                table: "ApplicationState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolarSettings",
                table: "ApplicationState",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthSettings",
                table: "ApplicationState");

            migrationBuilder.DropColumn(
                name: "ShellySettings",
                table: "ApplicationState");

            migrationBuilder.DropColumn(
                name: "SolarSettings",
                table: "ApplicationState");
        }
    }
}
