using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadHub.Libro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanDb_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedQuantity",
                table: "LoanItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReturnedQuantity",
                table: "LoanItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
