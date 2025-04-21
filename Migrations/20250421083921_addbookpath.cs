using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookClub.Migrations
{
    /// <inheritdoc />
    public partial class addbookpath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bookpath",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bookpath",
                table: "Book");
        }
    }
}
