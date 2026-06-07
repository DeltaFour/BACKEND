using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class settingfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0040c417-becf-4f7e-a15c-488879583095"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("22cd6eba-64a8-4283-afe4-9a5acb278fd2"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("cd3eeccb-1501-48fc-9865-b04cf06b7eed"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("dbc8f1c2-13ef-468b-8b0d-5ec47a223dee"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("15a63d83-f349-44de-a2e3-de287e119ece"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("19a4c512-6fb8-4470-a69f-1a523bef393e"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("86e2ff65-f489-43a4-8b73-b55d7791206c"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 21, DateTimeKind.Utc).AddTicks(7199),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 19, DateTimeKind.Utc).AddTicks(8734),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 20, DateTimeKind.Utc).AddTicks(2445),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(9846),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 19, DateTimeKind.Utc).AddTicks(4290),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(2814),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(6859),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 17, DateTimeKind.Utc).AddTicks(4568),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 17, DateTimeKind.Utc).AddTicks(526),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 709, DateTimeKind.Utc).AddTicks(6570));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 16, DateTimeKind.Utc).AddTicks(2809),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 14, DateTimeKind.Utc).AddTicks(9404),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1cad664e-c93d-4918-89d5-d94bed6d9685"), "create" },
                    { new Guid("22f94e9f-a5e0-4a72-be14-25728affa15d"), "delete" },
                    { new Guid("51f07354-c47b-416b-a30d-39510b2968b4"), "update" },
                    { new Guid("cfc43594-e5e1-4e96-a1bb-2edeb93efeb4"), "list" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("4e2aa4a5-b6ca-4c15-a31c-e75ba72771f7"), "work" },
                    { new Guid("5d4f624b-3410-4d1e-9302-d6325f61d842"), "employee" },
                    { new Guid("e1b48b2a-bb52-4938-9c8c-25d7e7c05735"), "company" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1cad664e-c93d-4918-89d5-d94bed6d9685"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("22f94e9f-a5e0-4a72-be14-25728affa15d"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("51f07354-c47b-416b-a30d-39510b2968b4"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("cfc43594-e5e1-4e96-a1bb-2edeb93efeb4"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("4e2aa4a5-b6ca-4c15-a31c-e75ba72771f7"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("5d4f624b-3410-4d1e-9302-d6325f61d842"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e1b48b2a-bb52-4938-9c8c-25d7e7c05735"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 21, DateTimeKind.Utc).AddTicks(7199));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 19, DateTimeKind.Utc).AddTicks(8734));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 20, DateTimeKind.Utc).AddTicks(2445));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(9846));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 19, DateTimeKind.Utc).AddTicks(4290));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(2814));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 18, DateTimeKind.Utc).AddTicks(6859));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 17, DateTimeKind.Utc).AddTicks(4568));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 709, DateTimeKind.Utc).AddTicks(6570),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 17, DateTimeKind.Utc).AddTicks(526));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 16, DateTimeKind.Utc).AddTicks(2809));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 18, 8, 39, 14, DateTimeKind.Utc).AddTicks(9404));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0040c417-becf-4f7e-a15c-488879583095"), "update" },
                    { new Guid("22cd6eba-64a8-4283-afe4-9a5acb278fd2"), "delete" },
                    { new Guid("cd3eeccb-1501-48fc-9865-b04cf06b7eed"), "list" },
                    { new Guid("dbc8f1c2-13ef-468b-8b0d-5ec47a223dee"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("15a63d83-f349-44de-a2e3-de287e119ece"), "work" },
                    { new Guid("19a4c512-6fb8-4470-a69f-1a523bef393e"), "employee" },
                    { new Guid("86e2ff65-f489-43a4-8b73-b55d7791206c"), "company" }
                });
        }
    }
}
