using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadHub.Libro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExpiredEmailSent",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderEmailSent",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredEmailSent",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ReminderEmailSent",
                table: "Loans");
        }
    }
}
