using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegorTelegramBotListeningServices.Migrations
{
    public partial class AddTokenCaching : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotTokenCache",
                table: "Webhooks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotTokenCache",
                table: "Webhooks");
        }
    }
}
