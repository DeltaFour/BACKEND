using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
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
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 37, DateTimeKind.Utc).AddTicks(4944),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(7620),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 36, DateTimeKind.Utc).AddTicks(1458),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(7741),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(3559),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(287),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(4645),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 33, DateTimeKind.Utc).AddTicks(1903),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(8141),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 709, DateTimeKind.Utc).AddTicks(6570));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 31, DateTimeKind.Utc).AddTicks(5995),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 30, DateTimeKind.Utc).AddTicks(3455),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248));

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    severity = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reference_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(4470))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_notifications_company_id_created_at",
                table: "notifications",
                columns: new[] { "company_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

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
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 37, DateTimeKind.Utc).AddTicks(4944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(7620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 36, DateTimeKind.Utc).AddTicks(1458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(7741));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 35, DateTimeKind.Utc).AddTicks(3559));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(287));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 34, DateTimeKind.Utc).AddTicks(4645));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 33, DateTimeKind.Utc).AddTicks(1903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "password_reset_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 709, DateTimeKind.Utc).AddTicks(6570),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 32, DateTimeKind.Utc).AddTicks(8141));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 31, DateTimeKind.Utc).AddTicks(5995));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 7, 16, 40, 43, 30, DateTimeKind.Utc).AddTicks(3455));

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
