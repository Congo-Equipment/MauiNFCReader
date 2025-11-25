using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NfcReader.Backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameColInTabRecording_and_AddTabClockingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "T_MOBILE_BADGE_INFO",
                newName: "CREATED");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "T_MOBILE_BADGE_INFO",
                newName: "STAFF_ID");

            migrationBuilder.RenameColumn(
                name: "BadgeId",
                table: "T_MOBILE_BADGE_INFO",
                newName: "BADGE_ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "T_MOBILE_BADGE_INFO",
                newName: "OID");

            migrationBuilder.AddColumn<Guid>(
                name: "OID_TYPE_POINTAGE",
                table: "T_POINTAGE_TRAV_MOBILE",
                type: "uniqueidentifier",
                nullable: true,
                defaultValue: null);

            migrationBuilder.CreateTable(
                name: "T_CLOCKING_TYPE",
                columns: table => new
                {
                    OID_TYPE = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CREATED = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CLOCKING_TYPE", x => x.OID_TYPE);
                });

            migrationBuilder.Sql(@"
                INSERT INTO T_CLOCKING_TYPE (OID_TYPE, NAME, DESCRIPTION, CREATED, UPDATED)
                VALUES ('00000000-0000-0000-0000-000000000000', 'LEGACY_DEFAULT', 'Auto-generated for existing records', GETDATE(), GETDATE())
            ");

            migrationBuilder.CreateIndex(
                name: "IX_T_POINTAGE_TRAV_MOBILE_OID_TYPE_POINTAGE",
                table: "T_POINTAGE_TRAV_MOBILE",
                column: "OID_TYPE_POINTAGE");

            migrationBuilder.CreateIndex(
                name: "IX_T_CLOCKING_TYPE_NAME",
                table: "T_CLOCKING_TYPE",
                column: "NAME",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE",
                column: "OID_TYPE_POINTAGE",
                principalTable: "T_CLOCKING_TYPE",
                principalColumn: "OID_TYPE",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawClocking_ClockingType",
                table: "T_POINTAGE_TRAV_MOBILE");

            migrationBuilder.DropTable(
                name: "T_CLOCKING_TYPE");

            migrationBuilder.DropIndex(
                name: "IX_T_POINTAGE_TRAV_MOBILE_OID_TYPE_POINTAGE",
                table: "T_POINTAGE_TRAV_MOBILE");

            migrationBuilder.DropColumn(
                name: "OID_TYPE_POINTAGE",
                table: "T_POINTAGE_TRAV_MOBILE");

            migrationBuilder.RenameColumn(
                name: "CREATED",
                table: "T_MOBILE_BADGE_INFO",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "STAFF_ID",
                table: "T_MOBILE_BADGE_INFO",
                newName: "StaffId");

            migrationBuilder.RenameColumn(
                name: "BADGE_ID",
                table: "T_MOBILE_BADGE_INFO",
                newName: "BadgeId");

            migrationBuilder.RenameColumn(
                name: "OID",
                table: "T_MOBILE_BADGE_INFO",
                newName: "Id");
        }
    }
}
