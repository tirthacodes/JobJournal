using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobJournal.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoDataToCvProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "CvProfiles");

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "CvProfiles",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "CvProfiles");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "CvProfiles",
                type: "TEXT",
                maxLength: 250,
                nullable: true);
        }
    }
}
