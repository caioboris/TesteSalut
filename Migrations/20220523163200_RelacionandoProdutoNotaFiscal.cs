using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesteSalut.Migrations
{
    public partial class RelacionandoProdutoNotaFiscal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_NotaFiscal_NotaFiscalId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_NotaFiscalId",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "NotaFiscalId",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "UploadCupomFiscal",
                table: "NotaFiscal");

            migrationBuilder.AddColumn<double>(
                name: "ValorTotal",
                table: "NotaFiscal",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ProdutoNotaFiscal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    NotaFiscalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoNotaFiscal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoNotaFiscal_NotaFiscal_NotaFiscalId",
                        column: x => x.NotaFiscalId,
                        principalTable: "NotaFiscal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProdutoNotaFiscal_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoNotaFiscal_NotaFiscalId",
                table: "ProdutoNotaFiscal",
                column: "NotaFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoNotaFiscal_ProdutoId",
                table: "ProdutoNotaFiscal",
                column: "ProdutoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutoNotaFiscal");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "NotaFiscal");

            migrationBuilder.AddColumn<Guid>(
                name: "NotaFiscalId",
                table: "Produto",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadCupomFiscal",
                table: "NotaFiscal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_NotaFiscalId",
                table: "Produto",
                column: "NotaFiscalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_NotaFiscal_NotaFiscalId",
                table: "Produto",
                column: "NotaFiscalId",
                principalTable: "NotaFiscal",
                principalColumn: "Id");
        }
    }
}
