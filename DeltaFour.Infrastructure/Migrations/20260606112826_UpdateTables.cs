using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 370, DateTimeKind.Utc).AddTicks(1992),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 936, DateTimeKind.Utc).AddTicks(1369));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(8293),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(7835));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(9934),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(9908));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(4181),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(4062));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(1771),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(1792));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 366, DateTimeKind.Utc).AddTicks(4232),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 932, DateTimeKind.Utc).AddTicks(4584));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 365, DateTimeKind.Utc).AddTicks(1889),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 931, DateTimeKind.Utc).AddTicks(2589));

            migrationBuilder.AddColumn<string>(
                name: "legal_name",
                table: "company",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "address",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("06e6ac39-08cd-4d7e-b945-d9d1e43bc9b5"), "list" },
                    { new Guid("4756702d-5682-4af9-8e1e-da9c5b6a2b9f"), "update" },
                    { new Guid("52786235-4e6c-4a26-8787-ca89888fb342"), "create" },
                    { new Guid("a038204d-7da8-4ced-92e5-c8584b4e51e4"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("858d8787-70dc-48d1-ada1-09fc763069f1"), "work" },
                    { new Guid("a06cca9d-fe3c-42db-a4f0-d76248e2a972"), "company" },
                    { new Guid("d5503dbb-0697-43fb-bb2a-5b433b3884a3"), "employee" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("06e6ac39-08cd-4d7e-b945-d9d1e43bc9b5"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4756702d-5682-4af9-8e1e-da9c5b6a2b9f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("52786235-4e6c-4a26-8787-ca89888fb342"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("a038204d-7da8-4ced-92e5-c8584b4e51e4"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("858d8787-70dc-48d1-ada1-09fc763069f1"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("a06cca9d-fe3c-42db-a4f0-d76248e2a972"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("d5503dbb-0697-43fb-bb2a-5b433b3884a3"));

            migrationBuilder.DropColumn(
                name: "legal_name",
                table: "company");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "address");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 936, DateTimeKind.Utc).AddTicks(1369),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 370, DateTimeKind.Utc).AddTicks(1992));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(7835),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(8293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(9908),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(9934));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 934, DateTimeKind.Utc).AddTicks(4062),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(4181));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 933, DateTimeKind.Utc).AddTicks(1792),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(1771));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 932, DateTimeKind.Utc).AddTicks(4584),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 366, DateTimeKind.Utc).AddTicks(4232));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 2, 23, 55, 33, 931, DateTimeKind.Utc).AddTicks(2589),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 365, DateTimeKind.Utc).AddTicks(1889));

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
        }
    }
}
