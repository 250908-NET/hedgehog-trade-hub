using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTradeModelAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Trades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
