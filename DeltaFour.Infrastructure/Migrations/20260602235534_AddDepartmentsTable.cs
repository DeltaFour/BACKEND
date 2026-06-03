using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1fee153c-4be3-4442-82a0-c92b8b5d9812"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("216a8a62-1b84-474b-9edf-a4e88a2b2836"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("38e7ec04-61b1-44e9-bff4-8d51b90fb1de"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("df10675d-1c11-47dd-b03b-84dd28a2b40d"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("1c89625d-6c94-41bd-ae16-e9593cbb1c84"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("da6a01a8-93bb-4785-9655-a471f2fa717f"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e71dfebf-e7dd-4bfd-9d30-9244c22839ec"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 936, DateTimeKind.Utc).AddTicks(1369),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 872, DateTimeKind.Utc).AddTicks(7286));

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "user",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(7835),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 871, DateTimeKind.Utc).AddTicks(3059));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(9908),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(4603));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(4062),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(8980));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(1792),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 869, DateTimeKind.Utc).AddTicks(6011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 931, DateTimeKind.Utc).AddTicks(2589),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 868, DateTimeKind.Utc).AddTicks(42));

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 932, DateTimeKind.Utc).AddTicks(4584))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1aeb408c-2b7e-49df-9f01-b8d7ae4d502e"), "update" },
                    { new Guid("1d365ec2-f93e-4877-943d-665a8d4064de"), "create" },
                    { new Guid("b14342fb-e615-4a34-8ba4-fee65152434e"), "delete" },
                    { new Guid("bcd16334-c144-4388-8e24-44e4af9b17ed"), "list" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("4f40a0e0-434e-449d-9866-0efaf1204aba"), "company" },
                    { new Guid("8d34b18a-a559-4721-9d3c-6fcd55a18cdf"), "work" },
                    { new Guid("e4a727bf-5556-432a-92f5-fead5e0b119b"), "employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_DepartmentId",
                table: "user",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_department_company_id",
                table: "department",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_department_DepartmentId",
                table: "user",
                column: "DepartmentId",
                principalTable: "department",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_department_DepartmentId",
                table: "user");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropIndex(
                name: "IX_user_DepartmentId",
                table: "user");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1aeb408c-2b7e-49df-9f01-b8d7ae4d502e"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1d365ec2-f93e-4877-943d-665a8d4064de"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("b14342fb-e615-4a34-8ba4-fee65152434e"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("bcd16334-c144-4388-8e24-44e4af9b17ed"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("4f40a0e0-434e-449d-9866-0efaf1204aba"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("8d34b18a-a559-4721-9d3c-6fcd55a18cdf"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e4a727bf-5556-432a-92f5-fead5e0b119b"));

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 872, DateTimeKind.Utc).AddTicks(7286),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 936, DateTimeKind.Utc).AddTicks(1369));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 871, DateTimeKind.Utc).AddTicks(3059),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(7835));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(4603),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(9908));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(8980),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(4062));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 869, DateTimeKind.Utc).AddTicks(6011),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(1792));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 868, DateTimeKind.Utc).AddTicks(42),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 931, DateTimeKind.Utc).AddTicks(2589));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1fee153c-4be3-4442-82a0-c92b8b5d9812"), "list" },
                    { new Guid("216a8a62-1b84-474b-9edf-a4e88a2b2836"), "update" },
                    { new Guid("38e7ec04-61b1-44e9-bff4-8d51b90fb1de"), "delete" },
                    { new Guid("df10675d-1c11-47dd-b03b-84dd28a2b40d"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1c89625d-6c94-41bd-ae16-e9593cbb1c84"), "company" },
                    { new Guid("da6a01a8-93bb-4785-9655-a471f2fa717f"), "employee" },
                    { new Guid("e71dfebf-e7dd-4bfd-9d30-9244c22839ec"), "work" }
                });
        }
    }
}
