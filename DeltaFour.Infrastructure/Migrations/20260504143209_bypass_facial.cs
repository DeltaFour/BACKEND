using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bypass_facial : Migration
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
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 902, DateTimeKind.Utc).AddTicks(87),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 516, DateTimeKind.Utc).AddTicks(7899));

            migrationBuilder.AddColumn<bool>(
                name: "is_allowed_bypass_facial",
                table: "user",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 900, DateTimeKind.Utc).AddTicks(3570),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 514, DateTimeKind.Utc).AddTicks(6679));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 899, DateTimeKind.Utc).AddTicks(105),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 512, DateTimeKind.Utc).AddTicks(8742));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("06f091b1-1605-4831-8adc-f5271613b35c"), "update" },
                    { new Guid("16967bfb-2095-46b2-8b56-091bcb6980ed"), "list" },
                    { new Guid("65026044-70d7-454a-aad8-04cede3df44d"), "delete" },
                    { new Guid("66809682-b09d-4f8a-8789-c48e48f65ea7"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("7469703e-97fb-4db8-b062-8146bd9c8ea9"), "employee" },
                    { new Guid("89411c00-6b2f-436d-84ee-74a397ec1db7"), "work" },
                    { new Guid("e0ecf18a-3259-4865-b716-fa654900450c"), "company" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("06f091b1-1605-4831-8adc-f5271613b35c"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("16967bfb-2095-46b2-8b56-091bcb6980ed"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("65026044-70d7-454a-aad8-04cede3df44d"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("66809682-b09d-4f8a-8789-c48e48f65ea7"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("7469703e-97fb-4db8-b062-8146bd9c8ea9"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("89411c00-6b2f-436d-84ee-74a397ec1db7"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e0ecf18a-3259-4865-b716-fa654900450c"));

            migrationBuilder.DropColumn(
                name: "is_allowed_bypass_facial",
                table: "user");

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
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 902, DateTimeKind.Utc).AddTicks(87));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 514, DateTimeKind.Utc).AddTicks(6679),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 900, DateTimeKind.Utc).AddTicks(3570));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 23, 19, 46, 55, 512, DateTimeKind.Utc).AddTicks(8742),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 899, DateTimeKind.Utc).AddTicks(105));

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
