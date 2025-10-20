using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAG_Code_Base.Migrations
{
    /// <inheritdoc />
    public partial class AddInfoBlocksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    BlockType = table.Column<string>(type: "text", nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: true),
                    MethodName = table.Column<string>(type: "text", nullable: true),
                    StartLine = table.Column<int>(type: "integer", nullable: false),
                    EndLine = table.Column<int>(type: "integer", nullable: false),
                    HeaderSection = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoBlocks_FileItems_FileItemId",
                        column: x => x.FileItemId,
                        principalTable: "FileItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfoBlocks_FileItemId",
                table: "InfoBlocks",
                column: "FileItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoBlocks");
        }
    }
}
