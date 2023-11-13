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
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClones", x => x.ID_User);
                });

            migrationBuilder.CreateTable(
                name: "DeviceInstances",
                columns: table => new
                {
                    ID_Dev_Inst = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dev_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_User = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID_Dev_Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInstances", x => x.ID_Dev_Inst);
                    table.ForeignKey(
                        name: "FK_DeviceInstances_UserClones_ID_User",
                        column: x => x.ID_User,
                        principalTable: "UserClones",
                        principalColumn: "ID_User",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTypes",
                columns: table => new
                {
                    ID_Dev_Type = table.Column<int>(type: "int", nullable: false),
                    Dev_Type_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxPower = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTypes", x => x.ID_Dev_Type);
                    table.ForeignKey(
                        name: "FK_DeviceTypes_DeviceInstances_ID_Dev_Type",
                        column: x => x.ID_Dev_Type,
                        principalTable: "DeviceInstances",
                        principalColumn: "ID_Dev_Inst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceInstances_ID_User",
                table: "DeviceInstances",
                column: "ID_User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceTypes");

            migrationBuilder.DropTable(
                name: "DeviceInstances");

            migrationBuilder.DropTable(
                name: "UserClones");
        }
    }
}
