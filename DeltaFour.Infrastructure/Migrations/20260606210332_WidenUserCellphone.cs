using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WidenUserCellphone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("01b60c35-6e90-49d2-8af7-0cb90e9117a4"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("47035f41-4202-49c5-be0e-a6938d3c90d5"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("473aa970-f287-43b9-90a0-81f27f3c959f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("50f6d788-e626-4d08-8bc4-2fe9b3d7c5b1"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("12f52819-22bc-4bec-b480-372dae718f60"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("6a9b7a02-56ff-442c-9c50-cafa7116baea"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("a2df60b7-6b4d-460c-822b-51a929dabdf1"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 871, DateTimeKind.Utc).AddTicks(5306),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 42, DateTimeKind.Utc).AddTicks(4262));

            migrationBuilder.AlterColumn<string>(
                name: "cellphone",
                table: "user",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(14)",
                oldMaxLength: 14,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(7055),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(4011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 870, DateTimeKind.Utc).AddTicks(823),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(8384));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(6931),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(3900));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(2443),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(9347));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(8497),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 38, DateTimeKind.Utc).AddTicks(5141));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(2903),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(64),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 37, DateTimeKind.Utc).AddTicks(5568));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 866, DateTimeKind.Utc).AddTicks(1088),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 36, DateTimeKind.Utc).AddTicks(6804));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 864, DateTimeKind.Utc).AddTicks(8642),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 35, DateTimeKind.Utc).AddTicks(2151));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("10bae8e1-a45c-4b15-9dda-82f8e13697d3"), "create" },
                    { new Guid("1dd3279c-eab7-4a97-b7ef-434f0c663471"), "update" },
                    { new Guid("636dbb48-da7e-4f0d-80d4-d484703dfe02"), "delete" },
                    { new Guid("65bbf980-0478-4437-aaac-ef10bcbaa742"), "list" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0d876367-6432-47db-ac9b-29e54b6ef18f"), "work" },
                    { new Guid("5cd6f778-e111-4716-8cf5-565294f1b0e2"), "company" },
                    { new Guid("5f7226d7-59c9-48dd-9fd5-6eb1c004936a"), "employee" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("10bae8e1-a45c-4b15-9dda-82f8e13697d3"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1dd3279c-eab7-4a97-b7ef-434f0c663471"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("636dbb48-da7e-4f0d-80d4-d484703dfe02"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("65bbf980-0478-4437-aaac-ef10bcbaa742"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("0d876367-6432-47db-ac9b-29e54b6ef18f"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("5cd6f778-e111-4716-8cf5-565294f1b0e2"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("5f7226d7-59c9-48dd-9fd5-6eb1c004936a"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 42, DateTimeKind.Utc).AddTicks(4262),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 871, DateTimeKind.Utc).AddTicks(5306));

            migrationBuilder.AlterColumn<string>(
                name: "cellphone",
                table: "user",
                type: "varchar(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(4011),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(7055));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(8384),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 870, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(3900),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(6931));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(9347),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(2443));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 38, DateTimeKind.Utc).AddTicks(5141),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(8497));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(265),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(2903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 37, DateTimeKind.Utc).AddTicks(5568),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(64));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 36, DateTimeKind.Utc).AddTicks(6804),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 866, DateTimeKind.Utc).AddTicks(1088));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 35, DateTimeKind.Utc).AddTicks(2151),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 864, DateTimeKind.Utc).AddTicks(8642));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("01b60c35-6e90-49d2-8af7-0cb90e9117a4"), "list" },
                    { new Guid("47035f41-4202-49c5-be0e-a6938d3c90d5"), "create" },
                    { new Guid("473aa970-f287-43b9-90a0-81f27f3c959f"), "update" },
                    { new Guid("50f6d788-e626-4d08-8bc4-2fe9b3d7c5b1"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("12f52819-22bc-4bec-b480-372dae718f60"), "company" },
                    { new Guid("6a9b7a02-56ff-442c-9c50-cafa7116baea"), "employee" },
                    { new Guid("a2df60b7-6b4d-460c-822b-51a929dabdf1"), "work" }
                });
        }
    }
}
