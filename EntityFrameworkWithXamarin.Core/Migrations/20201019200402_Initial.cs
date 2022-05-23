using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkWithXamarin.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MessageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reguests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RGSt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reguests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reguests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReguestId = table.Column<int>(nullable: false),
                    A = table.Column<string>(nullable: true),
                    Txt = table.Column<string>(nullable: true),
                    Cost = table.Column<string>(nullable: true),
                    Quele = table.Column<string>(nullable: true),
                    Locacion = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableItems_Reguests_ReguestId",
                        column: x => x.ReguestId,
                        principalTable: "Reguests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reguests_UserId",
                table: "Reguests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TableItems_ReguestId",
                table: "TableItems",
                column: "ReguestId");

            migrationBuilder.CreateIndex(
                name: "IX_TableItems_UserId",
                table: "TableItems",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "TableItems");

            migrationBuilder.DropTable(
                name: "Reguests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
