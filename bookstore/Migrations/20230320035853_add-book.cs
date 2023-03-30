using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bookstore.Migrations
{
    public partial class addbook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isbn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numPages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    layout = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    publishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    view = table.Column<int>(type: "int", nullable: false),
                    weight = table.Column<float>(type: "real", nullable: false),
                    translatorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hSize = table.Column<float>(type: "real", nullable: false),
                    wSize = table.Column<float>(type: "real", nullable: false),
                    unitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unitStock = table.Column<int>(type: "int", nullable: false),
                    ranking = table.Column<float>(type: "real", nullable: false),
                    discount = table.Column<float>(type: "real", nullable: false),
                    cover = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    publisher_id = table.Column<int>(type: "int", nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    genre_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
