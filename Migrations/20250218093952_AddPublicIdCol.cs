using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatDongSan_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "PropertyImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "PropertyImages");
        }
    }
}
