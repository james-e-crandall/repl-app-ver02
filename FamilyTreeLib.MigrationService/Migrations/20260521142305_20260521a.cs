using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.SqlServer.Types;

#nullable disable

namespace FamilyTreeLib.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class _20260521a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halflings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PathFromPatriarch = table.Column<SqlHierarchyId>(type: "hierarchyid", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfBirth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halflings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Halflings");
        }
    }
}
