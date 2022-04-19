using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddOtherTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "client_secret",
                schema: "data",
                table: "app_user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deposit",
                schema: "data",
                table: "app_user",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount_used",
                schema: "data",
                table: "app_user",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "transaction",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    bank_account = table.Column<string>(type: "text", nullable: true),
                    completed_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    response = table.Column<string>(type: "text", nullable: true),
                    payment_gateway = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transaction", x => x.id);
                    table.ForeignKey(
                        name: "fk_transaction_users_app_user_id",
                        column: x => x.user_id,
                        principalSchema: "data",
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "web_configuration",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    api_secret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    web_name = table.Column<string>(type: "text", nullable: true),
                    endpoint = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_updated_by = table.Column<string>(type: "text", nullable: true),
                    last_updated_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_web_configuration", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transaction_user_id",
                schema: "data",
                table: "transaction",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaction",
                schema: "data");

            migrationBuilder.DropTable(
                name: "web_configuration",
                schema: "data");

            migrationBuilder.DropColumn(
                name: "client_secret",
                schema: "data",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "deposit",
                schema: "data",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "total_amount_used",
                schema: "data",
                table: "app_user");
        }
    }
}
