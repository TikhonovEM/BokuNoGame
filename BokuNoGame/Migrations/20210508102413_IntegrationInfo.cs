using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BokuNoGame.Migrations
{
    public partial class IntegrationInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "IntegrationInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalSystemDescriptor = table.Column<string>(nullable: true),
                    ExternalGameId = table.Column<int>(nullable: false),
                    InternalGameId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationInfos", x => x.Id);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationInfos");
        }
    }
}
