using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddWebTypeInWebConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "web_type",
                schema: "data",
                table: "web_configuration",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "web_type",
                schema: "data",
                table: "web_configuration");
        }
    }
}
