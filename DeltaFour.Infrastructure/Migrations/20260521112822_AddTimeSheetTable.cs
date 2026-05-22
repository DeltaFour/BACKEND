using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSheetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("36074c4a-2ec3-4a8e-99fb-091712ebcefb"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("55f50323-9b84-4973-a494-a3fd2d708843"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("cb860e55-01bb-4c86-b20e-9b8a6c73458c"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("d6079355-dd34-4ea8-90ae-6c21e5112991"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("3d1f5ca2-8eb3-4281-901c-b86ad906739d"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("4eb3eb82-50d9-407d-ab1f-2600d625c5ef"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("9a5bee31-154d-4ae3-a9a6-70ff8a96c9dc"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 40, DateTimeKind.Utc).AddTicks(7295),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 883, DateTimeKind.Utc).AddTicks(7079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(5065),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 882, DateTimeKind.Utc).AddTicks(1175));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(9460),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 882, DateTimeKind.Utc).AddTicks(5202));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 37, DateTimeKind.Utc).AddTicks(6884),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 881, DateTimeKind.Utc).AddTicks(3885));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 36, DateTimeKind.Utc).AddTicks(1344),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 879, DateTimeKind.Utc).AddTicks(8907));

            migrationBuilder.CreateTable(
                name: "time_sheet",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    month = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    signed_by_employee = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    employee_signed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    signed_by_hr = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    hr_signed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    signed_by_hr_user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    signed_by_hr_user_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 39, DateTimeKind.Utc).AddTicks(3412))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_sheet", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_sheet_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("190deee0-6846-47b2-b128-cf8e82f86337"), "delete" },
                    { new Guid("432aa524-15f6-4995-8aa3-36d7dc8ad2e5"), "list" },
                    { new Guid("71e2c264-4e0b-4f77-88ab-e6ba5290a5bf"), "update" },
                    { new Guid("b3b9322f-c361-4254-ae00-20eeddb8698a"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("25154d0f-7a14-4c2a-93ee-649c5c12dede"), "work" },
                    { new Guid("a77ad8ef-d3e6-413b-a18e-3275643f68e3"), "employee" },
                    { new Guid("e1a9bfc6-c8e5-4f11-afb9-aa5d93a89aae"), "company" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_time_sheet_user_month_year",
                table: "time_sheet",
                columns: new[] { "user_id", "month", "year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_sheet");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("190deee0-6846-47b2-b128-cf8e82f86337"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("432aa524-15f6-4995-8aa3-36d7dc8ad2e5"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("71e2c264-4e0b-4f77-88ab-e6ba5290a5bf"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("b3b9322f-c361-4254-ae00-20eeddb8698a"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("25154d0f-7a14-4c2a-93ee-649c5c12dede"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("a77ad8ef-d3e6-413b-a18e-3275643f68e3"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e1a9bfc6-c8e5-4f11-afb9-aa5d93a89aae"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 883, DateTimeKind.Utc).AddTicks(7079),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 40, DateTimeKind.Utc).AddTicks(7295));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 882, DateTimeKind.Utc).AddTicks(1175),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(5065));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 882, DateTimeKind.Utc).AddTicks(5202),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(9460));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 881, DateTimeKind.Utc).AddTicks(3885),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 37, DateTimeKind.Utc).AddTicks(6884));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 879, DateTimeKind.Utc).AddTicks(8907),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 36, DateTimeKind.Utc).AddTicks(1344));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("36074c4a-2ec3-4a8e-99fb-091712ebcefb"), "update" },
                    { new Guid("55f50323-9b84-4973-a494-a3fd2d708843"), "delete" },
                    { new Guid("cb860e55-01bb-4c86-b20e-9b8a6c73458c"), "list" },
                    { new Guid("d6079355-dd34-4ea8-90ae-6c21e5112991"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("3d1f5ca2-8eb3-4281-901c-b86ad906739d"), "work" },
                    { new Guid("4eb3eb82-50d9-407d-ab1f-2600d625c5ef"), "employee" },
                    { new Guid("9a5bee31-154d-4ae3-a9a6-70ff8a96c9dc"), "company" }
                });
        }
    }
}
