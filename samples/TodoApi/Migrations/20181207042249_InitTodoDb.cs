using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreKit.Samples.TodoApi.Migrations
{
    public partial class InitTodoDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Projects",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Projects", x => x.Id); });

            migrationBuilder.CreateTable(
                "Tasks",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Order = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Completed = table.Column<bool>(nullable: true),
                    AuthorId = table.Column<Guid>(nullable: false),
                    AuthorName = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        "FK_Tasks_Projects_ProjectId",
                        x => x.ProjectId,
                        "Projects",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Tasks_ProjectId",
                "Tasks",
                "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Tasks");

            migrationBuilder.DropTable(
                "Projects");
        }
    }
}
