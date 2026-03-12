using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionDeDevoluciones.Migrations
{
    /// <inheritdoc />
    public partial class AddDescripcionToDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "DecisionesAdoptadas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "DecisionesAdoptadas");
        }
    }
}
