using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiCentral.API.Infrastructure.Migrations
{
    public partial class InitialDriverSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartingLongitude",
                table: "Rides",
                newName: "TargetStartingLongitude");

            migrationBuilder.RenameColumn(
                name: "StartingLatitude",
                table: "Rides",
                newName: "TargetStartingLatitude");

            migrationBuilder.RenameColumn(
                name: "DestinationLongitude",
                table: "Rides",
                newName: "TargetDestinationLongitude");

            migrationBuilder.RenameColumn(
                name: "DestinationLatitude",
                table: "Rides",
                newName: "TargetDestinationLatitude");

            migrationBuilder.AlterColumn<double>(
                name: "TargetStartingLongitude",
                table: "Rides",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "TargetStartingLatitude",
                table: "Rides",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "ActualDestinationLatitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualDestinationLongitude",
                table: "Rides",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualStartingLatitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ActualStartingLongitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedTimeOfArrival",
                table: "Rides",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeOfArrival",
                table: "Rides",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("3886af86-a48a-402e-87ea-1c5412d09f93"), "Darko", "1234", "Meshkovski" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("3886af86-a48a-402e-87ea-1c5412d09f93"));

            migrationBuilder.DropColumn(
                name: "ActualDestinationLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualDestinationLongitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualStartingLatitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ActualStartingLongitude",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "EstimatedTimeOfArrival",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TimeOfArrival",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "TargetStartingLongitude",
                table: "Rides",
                newName: "StartingLongitude");

            migrationBuilder.RenameColumn(
                name: "TargetStartingLatitude",
                table: "Rides",
                newName: "StartingLatitude");

            migrationBuilder.RenameColumn(
                name: "TargetDestinationLongitude",
                table: "Rides",
                newName: "DestinationLongitude");

            migrationBuilder.RenameColumn(
                name: "TargetDestinationLatitude",
                table: "Rides",
                newName: "DestinationLatitude");

            migrationBuilder.AlterColumn<double>(
                name: "StartingLongitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "StartingLatitude",
                table: "Rides",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
