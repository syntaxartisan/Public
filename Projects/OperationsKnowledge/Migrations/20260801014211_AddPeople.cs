using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationsKnowledge.Migrations
{
    /// <inheritdoc />
    public partial class AddPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "OperationalSystems");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "OperationalSystems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationalSystems_OwnerId",
                table: "OperationalSystems",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationalSystems_People_OwnerId",
                table: "OperationalSystems",
                column: "OwnerId",
                principalTable: "People",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationalSystems_People_OwnerId",
                table: "OperationalSystems");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropIndex(
                name: "IX_OperationalSystems_OwnerId",
                table: "OperationalSystems");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "OperationalSystems");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "OperationalSystems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
