using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookClub.Migrations
{
    /// <inheritdoc />
    public partial class setpass_standardtodouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Pass_Standard",
                table: "Learn",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Pass_Standard",
                table: "Learn",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
