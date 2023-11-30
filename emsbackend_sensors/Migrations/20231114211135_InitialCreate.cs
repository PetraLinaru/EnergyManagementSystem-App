using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace emsbackend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserClones",
                columns: table => new
                {
                    ID_User = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID_Dev_Inst = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClones", x => x.ID_User);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Dev_Inst = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_User = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sensors_UserClones_ID_User",
                        column: x => x.ID_User,
                        principalTable: "UserClones",
                        principalColumn: "ID_User",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_ID_User",
                table: "Sensors",
                column: "ID_User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "UserClones");
        }
    }
}
