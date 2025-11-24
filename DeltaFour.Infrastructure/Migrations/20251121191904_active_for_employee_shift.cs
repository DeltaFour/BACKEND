using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class active_for_employee_shift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0b9ce114-7445-4311-b721-dc8fc54ae64c"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4d963213-ea6c-4062-924a-82cf051f1101"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("8d5a0bad-9fe6-4a5b-a2af-f3cfe8cf1b34"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("f120cc17-a615-463b-8377-6ecd91d2aa5b"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("228230ff-8bd3-413a-be32-bb0a3b55ccfe"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e24daba8-f87f-4c48-9688-2cdb78fe9c64"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("ffcd3365-6110-46f4-8fa3-cc13cbcd55df"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 115, DateTimeKind.Utc).AddTicks(277),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 119, DateTimeKind.Utc).AddTicks(7923));

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "employee_shift",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 113, DateTimeKind.Utc).AddTicks(1865),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 118, DateTimeKind.Utc).AddTicks(1248));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 110, DateTimeKind.Utc).AddTicks(9387),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 115, DateTimeKind.Utc).AddTicks(8936));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1060365c-087d-4716-85e5-85baeaa6ca9a"), "create" },
                    { new Guid("6a0276d3-32e7-4978-a394-ccd0bfb0de26"), "delete" },
                    { new Guid("72945b01-8317-40d6-bd84-029855d258f8"), "update" },
                    { new Guid("9c2c16d2-be40-46af-80c0-031d5bac74cf"), "list" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("2bf0488f-03ef-4c93-ada9-1376b96e0d39"), "work" },
                    { new Guid("2ca2b85b-03a8-4e35-b956-4d48ba222faf"), "company" },
                    { new Guid("b2a0b13f-1d15-445a-a716-9cb3f47f88d4"), "employee" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1060365c-087d-4716-85e5-85baeaa6ca9a"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("6a0276d3-32e7-4978-a394-ccd0bfb0de26"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("72945b01-8317-40d6-bd84-029855d258f8"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("9c2c16d2-be40-46af-80c0-031d5bac74cf"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("2bf0488f-03ef-4c93-ada9-1376b96e0d39"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("2ca2b85b-03a8-4e35-b956-4d48ba222faf"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("b2a0b13f-1d15-445a-a716-9cb3f47f88d4"));

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "employee_shift");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 119, DateTimeKind.Utc).AddTicks(7923),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 115, DateTimeKind.Utc).AddTicks(277));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 118, DateTimeKind.Utc).AddTicks(1248),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 113, DateTimeKind.Utc).AddTicks(1865));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 115, DateTimeKind.Utc).AddTicks(8936),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 19, 19, 4, 110, DateTimeKind.Utc).AddTicks(9387));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0b9ce114-7445-4311-b721-dc8fc54ae64c"), "create" },
                    { new Guid("4d963213-ea6c-4062-924a-82cf051f1101"), "list" },
                    { new Guid("8d5a0bad-9fe6-4a5b-a2af-f3cfe8cf1b34"), "delete" },
                    { new Guid("f120cc17-a615-463b-8377-6ecd91d2aa5b"), "update" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("228230ff-8bd3-413a-be32-bb0a3b55ccfe"), "work" },
                    { new Guid("e24daba8-f87f-4c48-9688-2cdb78fe9c64"), "employee" },
                    { new Guid("ffcd3365-6110-46f4-8fa3-cc13cbcd55df"), "company" }
                });
        }
    }
}
