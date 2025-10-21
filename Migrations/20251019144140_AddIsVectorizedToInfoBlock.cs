using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAG_Code_Base.Migrations
{
    /// <inheritdoc />
    public partial class AddIsVectorizedToInfoBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVectorized",
                table: "InfoBlocks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVectorized",
                table: "InfoBlocks");
        }
    }
}
