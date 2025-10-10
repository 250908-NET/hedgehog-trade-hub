using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTradeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<bool>(
                name: "InitiatedConfirmed",
                table: "Trades",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceivedConfirmed",
                table: "Trades",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatedConfirmed",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ReceivedConfirmed",
                table: "Trades");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Trades",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
