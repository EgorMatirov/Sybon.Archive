using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sybon.Archive.Migrations
{
    public partial class AddCachedProblemAndTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachedInternalProblems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InternalProblemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemoryLimitBytes = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatementUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestsCount = table.Column<int>(type: "int", nullable: false),
                    TimeLimitMillis = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedInternalProblems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CachedTests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProblemId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CachedTests_CachedInternalProblems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "CachedInternalProblems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedTests_ProblemId",
                table: "CachedTests",
                column: "ProblemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedTests");

            migrationBuilder.DropTable(
                name: "CachedInternalProblems");
        }
    }
}
