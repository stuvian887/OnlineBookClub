using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookClub.Migrations
{
    /// <inheritdoc />
    public partial class addChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Chapter_Id",
                table: "Learn",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Chapter",
                columns: table => new
                {
                    Chapter_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chapter_Index = table.Column<int>(type: "int", nullable: false),
                    Chapter_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Plan_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Chapter_Id);
                    table.ForeignKey(
                        name: "FK_Chapter_BookPlan_Plan_Id",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Learn_Chapter_Id",
                table: "Learn",
                column: "Chapter_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_Plan_Id",
                table: "Chapter",
                column: "Plan_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Learn_Chapter_Chapter_Id",
                table: "Learn",
                column: "Chapter_Id",
                principalTable: "Chapter",
                principalColumn: "Chapter_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Learn_Chapter_Chapter_Id",
                table: "Learn");

            migrationBuilder.DropTable(
                name: "Chapter");

            migrationBuilder.DropIndex(
                name: "IX_Learn_Chapter_Id",
                table: "Learn");

            migrationBuilder.DropColumn(
                name: "Chapter_Id",
                table: "Learn");
        }
    }
}
