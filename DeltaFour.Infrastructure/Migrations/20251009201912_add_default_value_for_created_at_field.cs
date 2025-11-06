using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_default_value_for_created_at_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 67, DateTimeKind.Utc).AddTicks(3271),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 65, DateTimeKind.Utc).AddTicks(6619),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 63, DateTimeKind.Utc).AddTicks(5341),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 67, DateTimeKind.Utc).AddTicks(3271));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 65, DateTimeKind.Utc).AddTicks(6619));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 10, 9, 20, 19, 11, 63, DateTimeKind.Utc).AddTicks(5341));
        }
    }
}
