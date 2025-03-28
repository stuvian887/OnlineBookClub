using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookClub.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AuthCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Members__206D917091A37B8D", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "BookPlan",
                columns: table => new
                {
                    Plan_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Plan_Goal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Plan_Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookPlan__9BAF9B03783099CE", x => x.Plan_Id);
                    table.ForeignKey(
                        name: "FK__BookPlan__User_I__3A81B327",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Notice",
                columns: table => new
                {
                    Notice_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    NoticeTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notice__E9930CABDF4AF963", x => x.Notice_Id);
                    table.ForeignKey(
                        name: "FK__Notice__User_Id__412EB0B6",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    Book_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Id = table.Column<int>(type: "int", nullable: false),
                    BookName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Book__C223F3B44BA23E96", x => x.Book_Id);
                    table.ForeignKey(
                        name: "FK__Book__Plan_Id__440B1D61",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id");
                });

            migrationBuilder.CreateTable(
                name: "Learn",
                columns: table => new
                {
                    Learn_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Id = table.Column<int>(type: "int", nullable: false),
                    Learn_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Learn_Index = table.Column<int>(type: "int", nullable: false),
                    Pass_Standard = table.Column<int>(type: "int", nullable: false),
                    DueTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Learn__319993003FCD92E2", x => x.Learn_Id);
                    table.ForeignKey(
                        name: "FK__Learn__Plan_Id__46E78A0C",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id");
                });

            migrationBuilder.CreateTable(
                name: "PlanMembers",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Plan_Id = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlanMemb__09D768C0FA70A000", x => new { x.User_Id, x.Plan_Id });
                    table.ForeignKey(
                        name: "FK__PlanMembe__Plan___5EBF139D",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id");
                    table.ForeignKey(
                        name: "FK__PlanMembe__User___5DCAEF64",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Post_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Img_Path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post__5875F7ADAF0F4B5B", x => x.Post_Id);
                    table.ForeignKey(
                        name: "FK__Post__Plan_Id__3D5E1FD2",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id");
                    table.ForeignKey(
                        name: "FK__Post__User_Id__3E52440B",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Statistic",
                columns: table => new
                {
                    Statistics_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Id = table.Column<int>(type: "int", nullable: false),
                    CopyCount = table.Column<int>(type: "int", nullable: false),
                    UserCount = table.Column<int>(type: "int", nullable: false),
                    ViewTimes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Statisti__A2EC2FD97545864C", x => x.Statistics_Id);
                    table.ForeignKey(
                        name: "FK__Statistic__Plan___6477ECF3",
                        column: x => x.Plan_Id,
                        principalTable: "BookPlan",
                        principalColumn: "Plan_Id");
                });

            migrationBuilder.CreateTable(
                name: "Answer_Record",
                columns: table => new
                {
                    AR_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Learn_Id = table.Column<int>(type: "int", nullable: false),
                    Topic_Id = table.Column<int>(type: "int", nullable: false),
                    AnswerDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    times = table.Column<int>(type: "int", nullable: false),
                    Pass = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Answer_R__003ED5F23A33418F", x => x.AR_Id);
                    table.ForeignKey(
                        name: "FK__Answer_Re__Learn__4E88ABD4",
                        column: x => x.Learn_Id,
                        principalTable: "Learn",
                        principalColumn: "Learn_Id");
                    table.ForeignKey(
                        name: "FK__Answer_Re__User___4D94879B",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProgressTracking",
                columns: table => new
                {
                    Progress_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Learn_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Progress__D558797A32C6845B", x => x.Progress_Id);
                    table.ForeignKey(
                        name: "FK__ProgressT__Learn__52593CB8",
                        column: x => x.Learn_Id,
                        principalTable: "Learn",
                        principalColumn: "Learn_Id");
                    table.ForeignKey(
                        name: "FK__ProgressT__User___5165187F",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Topic_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Learn_Id = table.Column<int>(type: "int", nullable: false),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Option_A = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Option_B = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Option_C = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Option_D = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Topic__8DEAA405DC674108", x => x.Topic_Id);
                    table.ForeignKey(
                        name: "FK__Topic__Learn_Id__619B8048",
                        column: x => x.Learn_Id,
                        principalTable: "Learn",
                        principalColumn: "Learn_Id");
                });

            migrationBuilder.CreateTable(
                name: "Post_Report",
                columns: table => new
                {
                    P_Report_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Post_Id = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "未審核"),
                    Report_text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Rep__AF4E25E378CB993A", x => x.P_Report_Id);
                    table.ForeignKey(
                        name: "FK__Post_Repo__Post___59FA5E80",
                        column: x => x.Post_Id,
                        principalTable: "Post",
                        principalColumn: "Post_Id");
                });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Reply_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Post_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    ReplyContent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReplyTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReplyImg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reply__B6633284EB09FCA7", x => x.Reply_Id);
                    table.ForeignKey(
                        name: "FK__Reply__Post_Id__49C3F6B7",
                        column: x => x.Post_Id,
                        principalTable: "Post",
                        principalColumn: "Post_Id");
                    table.ForeignKey(
                        name: "FK__Reply__User_Id__4AB81AF0",
                        column: x => x.User_Id,
                        principalTable: "Members",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Reply_Report",
                columns: table => new
                {
                    R_Report_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reply_Id = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "未審核"),
                    Report_text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reply_Re__6B66990F81BA8E1F", x => x.R_Report_Id);
                    table.ForeignKey(
                        name: "FK__Reply_Rep__Reply__5629CD9C",
                        column: x => x.Reply_Id,
                        principalTable: "Reply",
                        principalColumn: "Reply_Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_Record_Learn_Id",
                table: "Answer_Record",
                column: "Learn_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_Record_User_Id",
                table: "Answer_Record",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Plan_Id",
                table: "Book",
                column: "Plan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_BookPlan_User_Id",
                table: "BookPlan",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Learn_Plan_Id",
                table: "Learn",
                column: "Plan_Id");

            migrationBuilder.CreateIndex(
                name: "UQ__Members__A9D105349BEE5E48",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notice_User_Id",
                table: "Notice",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PlanMembers_Plan_Id",
                table: "PlanMembers",
                column: "Plan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Plan_Id",
                table: "Post",
                column: "Plan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_User_Id",
                table: "Post",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Report_Post_Id",
                table: "Post_Report",
                column: "Post_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressTracking_Learn_Id",
                table: "ProgressTracking",
                column: "Learn_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressTracking_User_Id",
                table: "ProgressTracking",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_Post_Id",
                table: "Reply",
                column: "Post_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_User_Id",
                table: "Reply",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_Report_Reply_Id",
                table: "Reply_Report",
                column: "Reply_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_Plan_Id",
                table: "Statistic",
                column: "Plan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_Learn_Id",
                table: "Topic",
                column: "Learn_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer_Record");

            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "Notice");

            migrationBuilder.DropTable(
                name: "PlanMembers");

            migrationBuilder.DropTable(
                name: "Post_Report");

            migrationBuilder.DropTable(
                name: "ProgressTracking");

            migrationBuilder.DropTable(
                name: "Reply_Report");

            migrationBuilder.DropTable(
                name: "Statistic");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropTable(
                name: "Learn");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "BookPlan");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
