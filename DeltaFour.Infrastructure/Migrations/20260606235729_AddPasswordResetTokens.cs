using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 871, DateTimeKind.Utc).AddTicks(5306));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(7055));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 870, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(6931));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(2443));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(8497));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(2903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(64));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 866, DateTimeKind.Utc).AddTicks(1088));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 864, DateTimeKind.Utc).AddTicks(8642));

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    code = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 709, DateTimeKind.Utc).AddTicks(6570))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_password_reset_tokens_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_user_id_code",
                table: "password_reset_tokens",
                columns: new[] { "user_id", "code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "password_reset_tokens");

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
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 871, DateTimeKind.Utc).AddTicks(5306),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 714, DateTimeKind.Utc).AddTicks(8061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signatures",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(7055),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(8729));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_signature_tokens",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 870, DateTimeKind.Utc).AddTicks(823),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 713, DateTimeKind.Utc).AddTicks(2907));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet_audits",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(6931),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(8657));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 869, DateTimeKind.Utc).AddTicks(2443),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 712, DateTimeKind.Utc).AddTicks(3913));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(8497),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(389));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 868, DateTimeKind.Utc).AddTicks(2903),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 711, DateTimeKind.Utc).AddTicks(5297));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 867, DateTimeKind.Utc).AddTicks(64),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 710, DateTimeKind.Utc).AddTicks(851));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 866, DateTimeKind.Utc).AddTicks(1088),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 708, DateTimeKind.Utc).AddTicks(8005));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 21, 3, 31, 864, DateTimeKind.Utc).AddTicks(8642),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 23, 57, 28, 707, DateTimeKind.Utc).AddTicks(2248));

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
    }
}
