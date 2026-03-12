using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionDeDevoluciones.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoArchivoToRemito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoRemito",
                table: "Remitos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "Remitos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionTexto",
                table: "Remitos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivoRemito",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "ObservacionTexto",
                table: "Remitos");
        }
    }
}
