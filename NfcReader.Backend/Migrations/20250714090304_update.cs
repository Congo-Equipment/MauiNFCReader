using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NfcReader.Backend.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BADGEID",
                table: "T_EMPLOYE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BADGEID",
                table: "T_EMPLOYE",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
