using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_subscription_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0a8fe140-db62-4ea2-b27e-3e4542cb4dde"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("2dc5cb63-334e-4891-af18-0cb3e9f8e632"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("99fdfc11-062b-45f4-9025-84e9c072ce34"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("b32dd941-d263-4ef2-9681-b0f9a74468e1"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("013b9de3-68ed-4ae1-9c94-7944034adc12"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("4ff04288-b269-47af-800b-0179da4cf630"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("b5f8205d-cb79-4df4-b67c-f5e5795f4f2a"));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "time_late",
                table: "user_attendance",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 573, DateTimeKind.Utc).AddTicks(4827),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 516, DateTimeKind.Utc).AddTicks(7899));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(1226),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 514, DateTimeKind.Utc).AddTicks(6679));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 569, DateTimeKind.Utc).AddTicks(6996),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 512, DateTimeKind.Utc).AddTicks(8742));

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    plan_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    external_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(9450))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscriptions_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscription_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    subscription_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    event_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payload = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 572, DateTimeKind.Utc).AddTicks(3529))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_events_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0738c524-0cdf-44d8-8e29-65f4204a0066"), "list" },
                    { new Guid("1e129f71-0fe0-42ae-bf41-b7188d868735"), "delete" },
                    { new Guid("e28af8b7-3b3e-4e8d-b1de-1522f8599855"), "create" },
                    { new Guid("e3885ee8-9a97-4fc2-83c0-76f07d77b372"), "update" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0abfe32e-c3df-4bc1-84d9-7054fd1637d8"), "work" },
                    { new Guid("69cca500-fc27-44f7-a81c-e5e88ea7a2a2"), "employee" },
                    { new Guid("ebf9d962-47f9-4cc6-bb1d-e196c66a631d"), "company" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_events_subscription_id",
                table: "subscription_events",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_company_id",
                table: "subscriptions",
                column: "company_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_events");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0738c524-0cdf-44d8-8e29-65f4204a0066"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1e129f71-0fe0-42ae-bf41-b7188d868735"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("e28af8b7-3b3e-4e8d-b1de-1522f8599855"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("e3885ee8-9a97-4fc2-83c0-76f07d77b372"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("0abfe32e-c3df-4bc1-84d9-7054fd1637d8"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("69cca500-fc27-44f7-a81c-e5e88ea7a2a2"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("ebf9d962-47f9-4cc6-bb1d-e196c66a631d"));

            migrationBuilder.AlterColumn<int>(
                name: "time_late",
                table: "user_attendance",
                type: "int",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 516, DateTimeKind.Utc).AddTicks(7899),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 573, DateTimeKind.Utc).AddTicks(4827));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 514, DateTimeKind.Utc).AddTicks(6679),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(1226));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 512, DateTimeKind.Utc).AddTicks(8742),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 569, DateTimeKind.Utc).AddTicks(6996));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0a8fe140-db62-4ea2-b27e-3e4542cb4dde"), "update" },
                    { new Guid("2dc5cb63-334e-4891-af18-0cb3e9f8e632"), "create" },
                    { new Guid("99fdfc11-062b-45f4-9025-84e9c072ce34"), "delete" },
                    { new Guid("b32dd941-d263-4ef2-9681-b0f9a74468e1"), "list" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("013b9de3-68ed-4ae1-9c94-7944034adc12"), "company" },
                    { new Guid("4ff04288-b269-47af-800b-0179da4cf630"), "employee" },
                    { new Guid("b5f8205d-cb79-4df4-b67c-f5e5795f4f2a"), "work" }
                });
        }
    }
}
