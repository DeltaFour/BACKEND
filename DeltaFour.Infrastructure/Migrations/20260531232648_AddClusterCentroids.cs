using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClusterCentroids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3113ffd3-b63c-4c4e-b819-70b55d6ab4d8"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4bf0529e-eed6-4275-b64d-23a0497651f7"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("bde2e118-498c-418e-9258-38c402ed3e3f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("e710810b-d6f0-4752-b769-a6d627262649"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("01cd51a7-2aad-4c77-aa48-42209a9b26da"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("684961b5-dff0-46d2-9020-520a6ed9a8d9"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("c0ab17de-1f38-4750-af96-70ac7efebcc7"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 872, DateTimeKind.Utc).AddTicks(7286),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 457, DateTimeKind.Utc).AddTicks(9130));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 871, DateTimeKind.Utc).AddTicks(3059),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(6021));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(4603),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 455, DateTimeKind.Utc).AddTicks(8033));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(8980),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(2126));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 869, DateTimeKind.Utc).AddTicks(6011),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 454, DateTimeKind.Utc).AddTicks(9788));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 868, DateTimeKind.Utc).AddTicks(42),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 453, DateTimeKind.Utc).AddTicks(5243));

            migrationBuilder.CreateTable(
                name: "cluster_centroids",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    cluster = table.Column<int>(type: "int", nullable: false),
                    late_percentage = table.Column<double>(type: "double", nullable: false),
                    average_late_minutes = table.Column<double>(type: "double", nullable: false),
                    max_late_minutes = table.Column<int>(type: "int", nullable: false),
                    total_absences = table.Column<int>(type: "int", nullable: false),
                    total_worked_days = table.Column<int>(type: "int", nullable: false),
                    calculated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cluster_centroids", x => x.id);
                    table.ForeignKey(
                        name: "FK_cluster_centroids_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1fee153c-4be3-4442-82a0-c92b8b5d9812"), "list" },
                    { new Guid("216a8a62-1b84-474b-9edf-a4e88a2b2836"), "update" },
                    { new Guid("38e7ec04-61b1-44e9-bff4-8d51b90fb1de"), "delete" },
                    { new Guid("df10675d-1c11-47dd-b03b-84dd28a2b40d"), "create" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("1c89625d-6c94-41bd-ae16-e9593cbb1c84"), "company" },
                    { new Guid("da6a01a8-93bb-4785-9655-a471f2fa717f"), "employee" },
                    { new Guid("e71dfebf-e7dd-4bfd-9d30-9244c22839ec"), "work" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cluster_centroids_company_id_cluster",
                table: "cluster_centroids",
                columns: new[] { "company_id", "cluster" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cluster_centroids");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("1fee153c-4be3-4442-82a0-c92b8b5d9812"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("216a8a62-1b84-474b-9edf-a4e88a2b2836"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("38e7ec04-61b1-44e9-bff4-8d51b90fb1de"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("df10675d-1c11-47dd-b03b-84dd28a2b40d"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("1c89625d-6c94-41bd-ae16-e9593cbb1c84"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("da6a01a8-93bb-4785-9655-a471f2fa717f"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e71dfebf-e7dd-4bfd-9d30-9244c22839ec"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 457, DateTimeKind.Utc).AddTicks(9130),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 872, DateTimeKind.Utc).AddTicks(7286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "time_sheet",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(6021),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 871, DateTimeKind.Utc).AddTicks(3059));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscriptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 455, DateTimeKind.Utc).AddTicks(8033),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(4603));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "subscription_events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 456, DateTimeKind.Utc).AddTicks(2126),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 870, DateTimeKind.Utc).AddTicks(8980));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 454, DateTimeKind.Utc).AddTicks(9788),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 869, DateTimeKind.Utc).AddTicks(6011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 31, 20, 3, 7, 453, DateTimeKind.Utc).AddTicks(5243),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 5, 31, 23, 26, 47, 868, DateTimeKind.Utc).AddTicks(42));

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("3113ffd3-b63c-4c4e-b819-70b55d6ab4d8"), "create" },
                    { new Guid("4bf0529e-eed6-4275-b64d-23a0497651f7"), "update" },
                    { new Guid("bde2e118-498c-418e-9258-38c402ed3e3f"), "list" },
                    { new Guid("e710810b-d6f0-4752-b769-a6d627262649"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("01cd51a7-2aad-4c77-aa48-42209a9b26da"), "work" },
                    { new Guid("684961b5-dff0-46d2-9020-520a6ed9a8d9"), "company" },
                    { new Guid("c0ab17de-1f38-4750-af96-70ac7efebcc7"), "employee" }
                });
        }
    }
}
