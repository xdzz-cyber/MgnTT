using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagniseTestTask.Migrations
{
    /// <inheritdoc />
    public partial class Initv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CryptoCurrencies",
                newName: "ShortName");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "CryptoCurrencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "CryptoCurrencies");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "CryptoCurrencies",
                newName: "Name");
        }
    }
}
