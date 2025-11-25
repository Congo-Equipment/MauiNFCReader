using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NfcReader.Backend.Migrations
{
    /// <inheritdoc />
    public partial class MinorUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE");

            migrationBuilder.AddForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE",
                column: "OID_TYPE_POINTAGE",
                principalTable: "T_CLOCKING_TYPE",
                principalColumn: "OID_TYPE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE");

            migrationBuilder.AddForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE",
                column: "OID_TYPE_POINTAGE",
                principalTable: "T_CLOCKING_TYPE",
                principalColumn: "OID_TYPE",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
