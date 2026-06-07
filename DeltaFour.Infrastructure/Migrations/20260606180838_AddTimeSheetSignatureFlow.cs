using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSheetSignatureFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 42, DateTimeKind.Utc).AddTicks(4262),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 370, DateTimeKind.Utc).AddTicks(1992));

            migrationBuilder.AddColumn<string>(
                name: "cpf",
                table: "user",
                type: "varchar(14)",
                unicode: false,
                maxLength: 14,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(9347),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(8293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 38, DateTimeKind.Utc).AddTicks(5141),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(9934));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(265),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(4181));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 37, DateTimeKind.Utc).AddTicks(5568),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(1771));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 36, DateTimeKind.Utc).AddTicks(6804),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 366, DateTimeKind.Utc).AddTicks(4232));

            migrationBuilder.AlterColumn<string>(
                name: "legal_name",
                table: "company",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 35, DateTimeKind.Utc).AddTicks(2151),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 365, DateTimeKind.Utc).AddTicks(1889));

            migrationBuilder.CreateTable(
                name: "time_sheet_audits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    time_sheet_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operation = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    old_values = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    new_values = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(3900))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_sheet_audits", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_sheet_audits_time_sheet_time_sheet_id",
                        column: x => x.time_sheet_id,
                        principalTable: "time_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "time_sheet_signature_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    time_sheet_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    signer_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signer_user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    token = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(8384))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_sheet_signature_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_sheet_signature_tokens_time_sheet_time_sheet_id",
                        column: x => x.time_sheet_id,
                        principalTable: "time_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "time_sheet_signatures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    time_sheet_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    signer_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signer_user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    signer_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signer_cpf = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signer_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signed_at_utc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    signer_ip = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time_sheet_hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 40, DateTimeKind.Utc).AddTicks(4011))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_sheet_signatures", x => x.id);
                    table.ForeignKey(
                        name: "FK_time_sheet_signatures_time_sheet_time_sheet_id",
                        column: x => x.time_sheet_id,
                        principalTable: "time_sheet",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_time_sheet_audits_time_sheet_id",
                table: "time_sheet_audits",
                column: "time_sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_sheet_signature_tokens_time_sheet_id",
                table: "time_sheet_signature_tokens",
                column: "time_sheet_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_sheet_signature_tokens_token",
                table: "time_sheet_signature_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_time_sheet_signatures_time_sheet_id",
                table: "time_sheet_signatures",
                column: "time_sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_sheet_audits");

            migrationBuilder.DropTable(
                name: "time_sheet_signature_tokens");

            migrationBuilder.DropTable(
                name: "time_sheet_signatures");

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

            migrationBuilder.DropColumn(
                name: "cpf",
                table: "user");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 370, DateTimeKind.Utc).AddTicks(1992),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 42, DateTimeKind.Utc).AddTicks(4262));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(8293),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(9347));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(9934),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 38, DateTimeKind.Utc).AddTicks(5141));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 368, DateTimeKind.Utc).AddTicks(4181),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 39, DateTimeKind.Utc).AddTicks(265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 367, DateTimeKind.Utc).AddTicks(1771),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 37, DateTimeKind.Utc).AddTicks(5568));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "department",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 366, DateTimeKind.Utc).AddTicks(4232),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 36, DateTimeKind.Utc).AddTicks(6804));

            migrationBuilder.UpdateData(
                table: "company",
                keyColumn: "legal_name",
                keyValue: null,
                column: "legal_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "legal_name",
                table: "company",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 6, 11, 28, 26, 365, DateTimeKind.Utc).AddTicks(1889),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 6, 6, 18, 8, 38, 35, DateTimeKind.Utc).AddTicks(2151));

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
    }
}
