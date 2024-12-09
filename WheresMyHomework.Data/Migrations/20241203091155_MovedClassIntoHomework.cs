using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheresMyHomework.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovedClassIntoHomework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_AspNetUsers_TeacherId",
                table: "HomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_HomeworkTasks_TeacherId",
                table: "HomeworkTasks");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "HomeworkTasks");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "HomeworkTasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "Classes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SchoolClassStudent",
                columns: table => new
                {
                    SchoolClassesId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClassStudent", x => new { x.SchoolClassesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_SchoolClassStudent_AspNetUsers_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolClassStudent_Classes_SchoolClassesId",
                        column: x => x.SchoolClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkTasks_ClassId",
                table: "HomeworkTasks",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassStudent_StudentsId",
                table: "SchoolClassStudent",
                column: "StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkTasks_Classes_ClassId",
                table: "HomeworkTasks",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_Classes_ClassId",
                table: "HomeworkTasks");

            migrationBuilder.DropTable(
                name: "SchoolClassStudent");

            migrationBuilder.DropIndex(
                name: "IX_HomeworkTasks_ClassId",
                table: "HomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "HomeworkTasks");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "HomeworkTasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkTasks_TeacherId",
                table: "HomeworkTasks",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkTasks_AspNetUsers_TeacherId",
                table: "HomeworkTasks",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
