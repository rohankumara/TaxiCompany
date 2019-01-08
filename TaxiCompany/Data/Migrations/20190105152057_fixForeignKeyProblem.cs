using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TaxiCompany.Data.Migrations
{
    public partial class fixForeignKeyProblem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Branch_BranchID",
                table: "Branch");

            migrationBuilder.DropIndex(
                name: "IX_Branch_BranchID",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Branch");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "Branch",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_BranchID",
                table: "Branch",
                column: "BranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Branch_BranchID",
                table: "Branch",
                column: "BranchID",
                principalTable: "Branch",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
