using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sybon.Archive.Migrations
{
    public partial class AddKeysBetweenCachedAndUsualProblem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InternalProblemId",
                table: "Problems",
                type: "nvarchar(450)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "InternalProblemId",
                table: "CachedInternalProblems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CachedInternalProblems_InternalProblemId",
                table: "CachedInternalProblems",
                column: "InternalProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_CachedInternalProblems_InternalProblemId",
                table: "Problems",
                column: "InternalProblemId",
                principalTable: "CachedInternalProblems",
                principalColumn: "InternalProblemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_CachedInternalProblems_InternalProblemId",
                table: "Problems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CachedInternalProblems_InternalProblemId",
                table: "CachedInternalProblems");

            migrationBuilder.AlterColumn<string>(
                name: "InternalProblemId",
                table: "Problems",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "InternalProblemId",
                table: "CachedInternalProblems",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
