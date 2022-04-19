using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddOtpCodeInOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transaction_users_app_user_id",
                schema: "data",
                table: "transaction");

            migrationBuilder.AddColumn<string>(
                name: "otp_code",
                schema: "data",
                table: "order_history",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_transaction_app_user_app_user_id",
                schema: "data",
                table: "transaction",
                column: "user_id",
                principalSchema: "data",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transaction_app_user_app_user_id",
                schema: "data",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "otp_code",
                schema: "data",
                table: "order_history");

            migrationBuilder.AddForeignKey(
                name: "fk_transaction_users_app_user_id",
                schema: "data",
                table: "transaction",
                column: "user_id",
                principalSchema: "data",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
