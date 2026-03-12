using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionDeDevoluciones.Migrations
{
    /// <inheritdoc />
    public partial class AddDecisionAndObservacionToRemito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DecisionesAdoptadas_Remitos_RemitoId",
                table: "DecisionesAdoptadas");

            migrationBuilder.DropForeignKey(
                name: "FK_Observaciones_Remitos_RemitoId",
                table: "Observaciones");

            migrationBuilder.DropIndex(
                name: "IX_Observaciones_RemitoId",
                table: "Observaciones");

            migrationBuilder.DropIndex(
                name: "IX_DecisionesAdoptadas_RemitoId",
                table: "DecisionesAdoptadas");

            migrationBuilder.DropColumn(
                name: "RemitoId",
                table: "Observaciones");

            migrationBuilder.DropColumn(
                name: "RemitoId",
                table: "DecisionesAdoptadas");

            migrationBuilder.AddColumn<int>(
                name: "DecisionAdoptadaId",
                table: "Remitos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Remitos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ObservacionId",
                table: "Remitos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Voltaje",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "RPM",
                table: "Productos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Protec",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroSerie",
                table: "Productos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Modelo",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Const",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "CV",
                table: "Productos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.CreateIndex(
                name: "IX_Remitos_DecisionAdoptadaId",
                table: "Remitos",
                column: "DecisionAdoptadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Remitos_ObservacionId",
                table: "Remitos",
                column: "ObservacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_DecisionesAdoptadas_DecisionAdoptadaId",
                table: "Remitos",
                column: "DecisionAdoptadaId",
                principalTable: "DecisionesAdoptadas",
                principalColumn: "DecisionAdoptadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Remitos_Observaciones_ObservacionId",
                table: "Remitos",
                column: "ObservacionId",
                principalTable: "Observaciones",
                principalColumn: "ObservacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_DecisionesAdoptadas_DecisionAdoptadaId",
                table: "Remitos");

            migrationBuilder.DropForeignKey(
                name: "FK_Remitos_Observaciones_ObservacionId",
                table: "Remitos");

            migrationBuilder.DropIndex(
                name: "IX_Remitos_DecisionAdoptadaId",
                table: "Remitos");

            migrationBuilder.DropIndex(
                name: "IX_Remitos_ObservacionId",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "DecisionAdoptadaId",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Remitos");

            migrationBuilder.DropColumn(
                name: "ObservacionId",
                table: "Remitos");

            migrationBuilder.AlterColumn<string>(
                name: "Voltaje",
                table: "Productos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RPM",
                table: "Productos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Protec",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroSerie",
                table: "Productos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Modelo",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Const",
                table: "Productos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CV",
                table: "Productos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemitoId",
                table: "Observaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemitoId",
                table: "DecisionesAdoptadas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Observaciones_RemitoId",
                table: "Observaciones",
                column: "RemitoId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesAdoptadas_RemitoId",
                table: "DecisionesAdoptadas",
                column: "RemitoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DecisionesAdoptadas_Remitos_RemitoId",
                table: "DecisionesAdoptadas",
                column: "RemitoId",
                principalTable: "Remitos",
                principalColumn: "RemitoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Observaciones_Remitos_RemitoId",
                table: "Observaciones",
                column: "RemitoId",
                principalTable: "Remitos",
                principalColumn: "RemitoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
