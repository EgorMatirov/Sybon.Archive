using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sybon.Archive.Migrations
{
    public partial class AddInputOutputFileNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InputFileName",
                table: "CachedInternalProblems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutputFileName",
                table: "CachedInternalProblems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputFileName",
                table: "CachedInternalProblems");

            migrationBuilder.DropColumn(
                name: "OutputFileName",
                table: "CachedInternalProblems");
        }
    }
}
