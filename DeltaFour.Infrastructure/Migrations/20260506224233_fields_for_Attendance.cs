using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fields_for_Attendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<bool>(
                name: "is_late",
                table: "user_attendance",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "user_attendance",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "justification",
                table: "user_attendance",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "observation",
                table: "user_attendance",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "user_attendance",
                type: "varchar(20)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 883, DateTimeKind.Utc).AddTicks(7079),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 902, DateTimeKind.Utc).AddTicks(87));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 881, DateTimeKind.Utc).AddTicks(3885),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 900, DateTimeKind.Utc).AddTicks(3570));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 879, DateTimeKind.Utc).AddTicks(8907),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 899, DateTimeKind.Utc).AddTicks(105));
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

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "user_attendance");

            migrationBuilder.DropColumn(
                name: "justification",
                table: "user_attendance");

            migrationBuilder.DropColumn(
                name: "observation",
                table: "user_attendance");

            migrationBuilder.DropColumn(
                name: "status",
                table: "user_attendance");

            migrationBuilder.AlterColumn<bool>(
                name: "is_late",
                table: "user_attendance",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 902, DateTimeKind.Utc).AddTicks(87),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 883, DateTimeKind.Utc).AddTicks(7079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 900, DateTimeKind.Utc).AddTicks(3570),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 881, DateTimeKind.Utc).AddTicks(3885));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 4, 14, 32, 8, 899, DateTimeKind.Utc).AddTicks(105),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 6, 22, 42, 32, 879, DateTimeKind.Utc).AddTicks(8907));

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
    }
}
