using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicApp.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Histories");
        }
    }
}
