using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobJournal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image",
                table: "JobInfos");

            migrationBuilder.CreateTable(
                name: "JobImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobInfoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageData = table.Column<string>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobImages_JobInfos_JobInfoId",
                        column: x => x.JobInfoId,
                        principalTable: "JobInfos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobImages_JobInfoId",
                table: "JobImages",
                column: "JobInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobImages");

            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "JobInfos",
                type: "TEXT",
                nullable: true);
        }
    }
}
