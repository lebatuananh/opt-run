using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddActionToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "action",
                schema: "data",
                table: "transaction",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "action",
                schema: "data",
                table: "transaction");
        }
    }
}
