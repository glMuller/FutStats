using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutStatsAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TIMES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    AnoFundacao = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIMES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JOGADORES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Numero = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Posicao = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TimeId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOGADORES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JOGADORES_TIMES_TimeId",
                        column: x => x.TimeId,
                        principalTable: "TIMES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JOGADORES_TIMEID",
                table: "JOGADORES",
                column: "TimeId");

            migrationBuilder.CreateIndex(
                name: "IX_TIMES_NOME",
                table: "TIMES",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOGADORES");

            migrationBuilder.DropTable(
                name: "TIMES");
        }
    }
}
