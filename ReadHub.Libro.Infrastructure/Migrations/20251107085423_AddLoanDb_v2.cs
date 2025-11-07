using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadHub.Libro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanDb_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedAt",
                table: "Loans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedAt",
                table: "Loans",
                type: "datetime2",
                nullable: true);
        }
    }
}
