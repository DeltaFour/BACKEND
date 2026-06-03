using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class criatabelasparametricaskmeans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 457, DateTimeKind.Utc).AddTicks(9130),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 40, DateTimeKind.Utc).AddTicks(7295));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(6021),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 39, DateTimeKind.Utc).AddTicks(3412));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 455, DateTimeKind.Utc).AddTicks(8033),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(5065));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(2126),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(9460));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 454, DateTimeKind.Utc).AddTicks(9788),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 37, DateTimeKind.Utc).AddTicks(6884));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 453, DateTimeKind.Utc).AddTicks(5243),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 36, DateTimeKind.Utc).AddTicks(1344));

            migrationBuilder.CreateTable(
                name: "user_punctuality_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    total_attendances = table.Column<int>(type: "int", nullable: false),
                    total_late_attendances = table.Column<int>(type: "int", nullable: false),
                    late_percentage = table.Column<double>(type: "double", nullable: false),
                    average_late_minutes = table.Column<double>(type: "double", nullable: false),
                    max_late_minutes = table.Column<int>(type: "int", nullable: false),
                    total_absences = table.Column<int>(type: "int", nullable: false),
                    total_worked_days = table.Column<int>(type: "int", nullable: false),
                    cluster = table.Column<int>(type: "int", nullable: true),
                    last_calculated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_punctuality_metrics", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_punctuality_metrics_user_user_id",
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
                    { new Guid("3113ffd3-b63c-4c4e-b819-70b55d6ab4d8"), "create" },
                    { new Guid("4bf0529e-eed6-4275-b64d-23a0497651f7"), "update" },
                    { new Guid("bde2e118-498c-418e-9258-38c402ed3e3f"), "list" },
                    { new Guid("e710810b-d6f0-4752-b769-a6d627262649"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("01cd51a7-2aad-4c77-aa48-42209a9b26da"), "work" },
                    { new Guid("684961b5-dff0-46d2-9020-520a6ed9a8d9"), "company" },
                    { new Guid("c0ab17de-1f38-4750-af96-70ac7efebcc7"), "employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_punctuality_metrics_user_id",
                table: "user_punctuality_metrics",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_punctuality_metrics");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3113ffd3-b63c-4c4e-b819-70b55d6ab4d8"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4bf0529e-eed6-4275-b64d-23a0497651f7"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("bde2e118-498c-418e-9258-38c402ed3e3f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("e710810b-d6f0-4752-b769-a6d627262649"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("01cd51a7-2aad-4c77-aa48-42209a9b26da"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("684961b5-dff0-46d2-9020-520a6ed9a8d9"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("c0ab17de-1f38-4750-af96-70ac7efebcc7"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 40, DateTimeKind.Utc).AddTicks(7295),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 457, DateTimeKind.Utc).AddTicks(9130));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 39, DateTimeKind.Utc).AddTicks(3412),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(6021));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(5065),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 455, DateTimeKind.Utc).AddTicks(8033));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 38, DateTimeKind.Utc).AddTicks(9460),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(2126));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 37, DateTimeKind.Utc).AddTicks(6884),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 454, DateTimeKind.Utc).AddTicks(9788));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 21, 11, 28, 22, 36, DateTimeKind.Utc).AddTicks(1344),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 453, DateTimeKind.Utc).AddTicks(5243));

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
        }
    }
}
