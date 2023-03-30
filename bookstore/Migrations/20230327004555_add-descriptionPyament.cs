using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bookstore.Migrations
{
    public partial class adddescriptionPyament : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "PaymentMethods");
        }
    }
}
