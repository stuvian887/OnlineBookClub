using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookClub.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Chapter_Name",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ChapterTopic",
                columns: table => new
                {
                    ChapterTopic_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chapter_Id = table.Column<int>(type: "int", nullable: false),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option_A = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option_B = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option_C = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Option_D = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterTopic", x => x.ChapterTopic_Id);
                    table.ForeignKey(
                        name: "FK_ChapterTopic_Chapter_Chapter_Id",
                        column: x => x.Chapter_Id,
                        principalTable: "Chapter",
                        principalColumn: "Chapter_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterTopic_Chapter_Id",
                table: "ChapterTopic",
                column: "Chapter_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterTopic");

            migrationBuilder.AlterColumn<string>(
                name: "Chapter_Name",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
