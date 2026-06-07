using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("29fb3896-dcc8-463f-b2d4-bea91140fef5"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("41ce06af-7e9a-4bbc-8568-cb2875d26164"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("9cf34f7f-df9f-4cd4-b7da-973855a23df5"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("c7a0daed-b3a6-4fdf-8297-bfb4e908175f"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("2bb658ad-2631-4b1c-85a2-335c2890275d"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("65a7ad6b-5c9d-4307-84df-528df5098902"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("77d1d4c4-7efd-4ca7-bfdb-e53fe67768c0"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 732, DateTimeKind.Utc).AddTicks(3915),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 37, DateTimeKind.Utc).AddTicks(4944));

            migrationBuilder.AddColumn<bool>(
                name: "must_change_password",
                table: "user",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(6214),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(7620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(9855),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 36, DateTimeKind.Utc).AddTicks(1458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 729, DateTimeKind.Utc).AddTicks(7266),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(7741));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(1962),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(3559));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 728, DateTimeKind.Utc).AddTicks(9694),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(287));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 729, DateTimeKind.Utc).AddTicks(4419),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(4645));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 728, DateTimeKind.Utc).AddTicks(1622),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 33, DateTimeKind.Utc).AddTicks(1903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 727, DateTimeKind.Utc).AddTicks(8074),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(8141));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "notifications",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 727, DateTimeKind.Utc).AddTicks(4536),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(4470));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 726, DateTimeKind.Utc).AddTicks(6650),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 31, DateTimeKind.Utc).AddTicks(5995));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 725, DateTimeKind.Utc).AddTicks(4156),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 30, DateTimeKind.Utc).AddTicks(3455));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("3596358e-09c5-4932-9441-2df0a44a3288"), "list" },
                    { new Guid("bf5f03bd-45b1-4afb-b286-a7531c0d972d"), "update" },
                    { new Guid("e4961484-b236-4c15-a22b-d66d51fe941d"), "create" },
                    { new Guid("efc48c6c-0551-423f-8b0e-ed41e9002c77"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("35ac6523-d88e-4cd4-ad36-a4ce71588093"), "employee" },
                    { new Guid("a4059ecd-9679-46de-86ea-528503b29add"), "company" },
                    { new Guid("c706c19d-79ae-4cb7-b541-93aa7517c8ea"), "work" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3596358e-09c5-4932-9441-2df0a44a3288"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("bf5f03bd-45b1-4afb-b286-a7531c0d972d"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("e4961484-b236-4c15-a22b-d66d51fe941d"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("efc48c6c-0551-423f-8b0e-ed41e9002c77"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("35ac6523-d88e-4cd4-ad36-a4ce71588093"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("a4059ecd-9679-46de-86ea-528503b29add"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("c706c19d-79ae-4cb7-b541-93aa7517c8ea"));

            migrationBuilder.DropColumn(
                name: "must_change_password",
                table: "user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 37, DateTimeKind.Utc).AddTicks(4944),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 732, DateTimeKind.Utc).AddTicks(3915));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(7620),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(6214));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 36, DateTimeKind.Utc).AddTicks(1458),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(9855));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(7741),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 729, DateTimeKind.Utc).AddTicks(7266));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(3559),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 730, DateTimeKind.Utc).AddTicks(1962));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(287),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 728, DateTimeKind.Utc).AddTicks(9694));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(4645),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 729, DateTimeKind.Utc).AddTicks(4419));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 33, DateTimeKind.Utc).AddTicks(1903),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 728, DateTimeKind.Utc).AddTicks(1622));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(8141),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 727, DateTimeKind.Utc).AddTicks(8074));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "notifications",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(4470),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 727, DateTimeKind.Utc).AddTicks(4536));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 31, DateTimeKind.Utc).AddTicks(5995),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 726, DateTimeKind.Utc).AddTicks(6650));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 30, DateTimeKind.Utc).AddTicks(3455),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 38, 57, 725, DateTimeKind.Utc).AddTicks(4156));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("29fb3896-dcc8-463f-b2d4-bea91140fef5"), "update" },
                    { new Guid("41ce06af-7e9a-4bbc-8568-cb2875d26164"), "list" },
                    { new Guid("9cf34f7f-df9f-4cd4-b7da-973855a23df5"), "delete" },
                    { new Guid("c7a0daed-b3a6-4fdf-8297-bfb4e908175f"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("2bb658ad-2631-4b1c-85a2-335c2890275d"), "work" },
                    { new Guid("65a7ad6b-5c9d-4307-84df-528df5098902"), "employee" },
                    { new Guid("77d1d4c4-7efd-4ca7-bfdb-e53fe67768c0"), "company" }
                });
        }
    }
}
