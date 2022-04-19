using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddRefToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ref",
                schema: "data",
                table: "transaction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ref",
                schema: "data",
                table: "transaction");
        }
    }
}
