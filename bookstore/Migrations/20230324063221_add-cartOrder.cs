using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bookstore.Migrations
{
    public partial class addcartOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    book_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    orderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total = table.Column<float>(type: "real", nullable: false),
                    discount = table.Column<float>(type: "real", nullable: false),
                    paymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    shippingMethod_id = table.Column<int>(type: "int", nullable: false),
                    paymentMethod_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    customerAddress_id = table.Column<int>(type: "int", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: false),
                    emloyee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
