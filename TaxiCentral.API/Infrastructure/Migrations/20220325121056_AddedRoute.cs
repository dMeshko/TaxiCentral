using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiCentral.API.Infrastructure.Migrations
{
    public partial class AddedRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Drivers_DriverId",
                table: "Rides");

            migrationBuilder.DropIndex(
                name: "IX_Rides_DriverId",
                table: "Rides");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("2867c64b-cbfa-46db-b88d-5c5bd6b8eac0"));

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TargetDestinationLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TargetDestinationLongitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TargetStartingLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TargetStartingLongitude",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "EstimatedTimeOfArrival",
                table: "Rides",
                newName: "WaitingTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetStartingLatitude = table.Column<double>(type: "float", nullable: false),
                    TargetStartingLongitude = table.Column<double>(type: "float", nullable: false),
                    TargetDestinationLatitude = table.Column<double>(type: "float", nullable: true),
                    TargetDestinationLongitude = table.Column<double>(type: "float", nullable: true),
                    ClientComment = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    DriverComment = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    EstimatedTimeOfArrival = table.Column<int>(type: "int", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VoidReason = table.Column<string>(type: "nvarchar(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Drivers_Id",
                        column: x => x.Id,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Routes_Rides_Id",
                        column: x => x.Id,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("500cc348-5244-4777-9ce2-94e3b1822789"), "Darko", "1234", "Meshkovski" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("500cc348-5244-4777-9ce2-94e3b1822789"));

            migrationBuilder.RenameColumn(
                name: "WaitingTime",
                table: "Rides",
                newName: "EstimatedTimeOfArrival");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Rides",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Rides",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId",
                table: "Rides",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "TargetDestinationLatitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetDestinationLongitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetStartingLatitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TargetStartingLongitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("2867c64b-cbfa-46db-b88d-5c5bd6b8eac0"), "Darko", "1234", "Meshkovski" });

            migrationBuilder.CreateIndex(
                name: "IX_Rides_DriverId",
                table: "Rides",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Drivers_DriverId",
                table: "Rides",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
