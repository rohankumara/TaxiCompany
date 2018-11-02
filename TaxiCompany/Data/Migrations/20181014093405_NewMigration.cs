using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TaxiCompany.Data.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Driver",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Driver",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Customer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Customer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Driver");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Customer");
        }
    }
}
