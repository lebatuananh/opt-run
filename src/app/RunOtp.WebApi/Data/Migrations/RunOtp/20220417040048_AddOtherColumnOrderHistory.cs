using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunOtp.WebApi.Data.Migrations.RunOtp
{
    public partial class AddOtherColumnOrderHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_time_response",
                schema: "data",
                table: "order_history",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "finish_time_response",
                schema: "data",
                table: "order_history",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_time_response",
                schema: "data",
                table: "order_history");

            migrationBuilder.DropColumn(
                name: "finish_time_response",
                schema: "data",
                table: "order_history");
        }
    }
}
