using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheresMyHomework.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_AspNetUsers_TeacherId1",
                table: "HomeworkTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentHomeworkTasks_AspNetUsers_StudentId1",
                table: "StudentHomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_StudentHomeworkTasks_StudentId1",
                table: "StudentHomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_HomeworkTasks_TeacherId1",
                table: "HomeworkTasks");

            migrationBuilder.DropColumn(
                name: "StudentId1",
                table: "StudentHomeworkTasks");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "HomeworkTasks");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "StudentHomeworkTasks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "HomeworkTasks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_StudentHomeworkTasks_StudentId",
                table: "StudentHomeworkTasks",
                column: "StudentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StudentHomeworkTasks_AspNetUsers_StudentId",
                table: "StudentHomeworkTasks",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_AspNetUsers_TeacherId",
                table: "HomeworkTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentHomeworkTasks_AspNetUsers_StudentId",
                table: "StudentHomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_StudentHomeworkTasks_StudentId",
                table: "StudentHomeworkTasks");

            migrationBuilder.DropIndex(
                name: "IX_HomeworkTasks_TeacherId",
                table: "HomeworkTasks");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentHomeworkTasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "StudentId1",
                table: "StudentHomeworkTasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "HomeworkTasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "TeacherId1",
                table: "HomeworkTasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentHomeworkTasks_StudentId1",
                table: "StudentHomeworkTasks",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkTasks_TeacherId1",
                table: "HomeworkTasks",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkTasks_AspNetUsers_TeacherId1",
                table: "HomeworkTasks",
                column: "TeacherId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentHomeworkTasks_AspNetUsers_StudentId1",
                table: "StudentHomeworkTasks",
                column: "StudentId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
