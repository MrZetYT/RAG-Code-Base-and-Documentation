using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAG_Code_Base.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndErrorToFileItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "FileItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FileItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "FileItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FileItems");
        }
    }
}
