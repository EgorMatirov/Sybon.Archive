using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sybon.Archive.Migrations
{
    public partial class Add_Global_Collection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Problems_InternalProblemId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "InternalProblemId",
                table: "Problems");

            migrationBuilder.AddColumn<long>(
                name: "GlobalProblemId",
                table: "Problems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "GlobalCollectionProblems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternalProblemId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalCollectionProblems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_GlobalProblemId",
                table: "Problems",
                column: "GlobalProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_GlobalCollectionProblems_GlobalProblemId",
                table: "Problems",
                column: "GlobalProblemId",
                principalTable: "GlobalCollectionProblems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_GlobalCollectionProblems_GlobalProblemId",
                table: "Problems");

            migrationBuilder.DropTable(
                name: "GlobalCollectionProblems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_GlobalProblemId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "GlobalProblemId",
                table: "Problems");

            migrationBuilder.AddColumn<string>(
                name: "InternalProblemId",
                table: "Problems",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_InternalProblemId",
                table: "Problems",
                column: "InternalProblemId");
        }
    }
}
