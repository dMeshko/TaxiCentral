using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiCentral.API.Infrastructure.Migrations
{
    public partial class UpdatedRouteRideAndDriverMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Drivers_Id",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Rides_Id",
                table: "Routes");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("500cc348-5244-4777-9ce2-94e3b1822789"));

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RideId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("bc5b9677-1d54-44c5-8d16-19c490dd04cb"), "Darko", "1234", "Meshkovski" });

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DriverId",
                table: "Routes",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RideId",
                table: "Routes",
                column: "RideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Drivers_DriverId",
                table: "Routes",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Rides_RideId",
                table: "Routes",
                column: "RideId",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Drivers_DriverId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Rides_RideId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_DriverId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_RideId",
                table: "Routes");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("bc5b9677-1d54-44c5-8d16-19c490dd04cb"));

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "RideId",
                table: "Routes");

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("500cc348-5244-4777-9ce2-94e3b1822789"), "Darko", "1234", "Meshkovski" });

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Drivers_Id",
                table: "Routes",
                column: "Id",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Rides_Id",
                table: "Routes",
                column: "Id",
                principalTable: "Rides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
