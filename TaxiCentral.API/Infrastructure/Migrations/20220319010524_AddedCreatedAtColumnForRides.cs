using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiCentral.API.Infrastructure.Migrations
{
    public partial class AddedCreatedAtColumnForRides : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("3886af86-a48a-402e-87ea-1c5412d09f93"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Rides",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("2867c64b-cbfa-46db-b88d-5c5bd6b8eac0"), "Darko", "1234", "Meshkovski" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("2867c64b-cbfa-46db-b88d-5c5bd6b8eac0"));

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rides");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Rides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "Name", "Pin", "Surname" },
                values: new object[] { new Guid("3886af86-a48a-402e-87ea-1c5412d09f93"), "Darko", "1234", "Meshkovski" });
        }
    }
}
