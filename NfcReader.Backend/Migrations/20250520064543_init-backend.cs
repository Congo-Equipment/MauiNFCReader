using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NfcReader.Backend.Migrations
{
    /// <inheritdoc />
    public partial class initbackend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_POINTAGE_TRAV_MOBILE",
                columns: table => new
                {
                    OID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BADGEID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OID_EMPLOYE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DATE_HEURE_POINTAGE = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false),
                    CREATED = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_POINTAGE_TRAV_MOBILE", x => x.OID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_POINTAGE_TRAV_MOBILE");
        }
    }
}
