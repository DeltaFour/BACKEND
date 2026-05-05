using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_customer_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 880, DateTimeKind.Utc).AddTicks(996),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 573, DateTimeKind.Utc).AddTicks(4827));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 878, DateTimeKind.Utc).AddTicks(5004),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(9450));

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "subscriptions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 878, DateTimeKind.Utc).AddTicks(9225),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 572, DateTimeKind.Utc).AddTicks(3529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 877, DateTimeKind.Utc).AddTicks(7040),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(1226));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 876, DateTimeKind.Utc).AddTicks(2090),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 569, DateTimeKind.Utc).AddTicks(6996));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1a59cb8b-f30d-4c50-9124-ad7b38e40a22"), "update" },
                    { new Guid("3728dd33-e9b5-446c-99e9-e4221467eac1"), "list" },
                    { new Guid("b8220605-5df2-45b3-bbd6-ee4e8cc3b667"), "create" },
                    { new Guid("f6ec583b-7f25-4121-bf1e-c9ef8db17e0a"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("103ad788-d192-4eff-bdf0-52d114258711"), "company" },
                    { new Guid("43ddc3a0-6680-444b-889e-9d2430c8d6b7"), "work" },
                    { new Guid("b8e74b7a-9763-4447-b65a-8a878bb5e57a"), "employee" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1a59cb8b-f30d-4c50-9124-ad7b38e40a22"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3728dd33-e9b5-446c-99e9-e4221467eac1"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("b8220605-5df2-45b3-bbd6-ee4e8cc3b667"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("f6ec583b-7f25-4121-bf1e-c9ef8db17e0a"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("103ad788-d192-4eff-bdf0-52d114258711"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("43ddc3a0-6680-444b-889e-9d2430c8d6b7"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("b8e74b7a-9763-4447-b65a-8a878bb5e57a"));

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "subscriptions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 573, DateTimeKind.Utc).AddTicks(4827),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 880, DateTimeKind.Utc).AddTicks(996));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(9450),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 878, DateTimeKind.Utc).AddTicks(5004));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 572, DateTimeKind.Utc).AddTicks(3529),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 878, DateTimeKind.Utc).AddTicks(9225));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 571, DateTimeKind.Utc).AddTicks(1226),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 877, DateTimeKind.Utc).AddTicks(7040));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 19, 23, 5, 2, 569, DateTimeKind.Utc).AddTicks(6996),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 27, 14, 45, 25, 876, DateTimeKind.Utc).AddTicks(2090));

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
        }
    }
}
