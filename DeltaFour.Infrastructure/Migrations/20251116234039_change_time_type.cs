using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class change_time_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0b355454-a2ee-4cab-b8dd-1838fef61b8a"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("101d686e-54ae-4628-a1dd-79d7dbda296f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("2d09d882-7fe5-4bee-8083-db08d6caa38f"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("84c692c7-e1ee-4e3a-af91-bb7dc3d783aa"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("29fc6aac-1abd-401d-a6be-ba42dff17435"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("6e2437a6-4e55-4d0e-baa2-97063f7287cb"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("8d9e390e-cd19-430c-a752-b38efe4bf611"));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "starter_time",
                table: "work_shift",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "end_time",
                table: "work_shift",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0692d812-087c-4290-82c5-582e3dff34ec"), "list" },
                    { new Guid("3d1cc1a8-b027-46d9-abbd-ee618d1c72a1"), "create" },
                    { new Guid("92ade242-36c1-4a85-9a7f-26576e318b5c"), "update" },
                    { new Guid("9c9cee76-6495-4806-8d15-34ce7a4fe100"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("44a1c0a0-644b-4751-8a78-89ee043adb84"), "employee" },
                    { new Guid("7b5d199e-7a5e-492a-a303-ae1fbfffdc3b"), "work" },
                    { new Guid("9639d831-4df1-4088-a36f-6e1a94cd35c5"), "company" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0692d812-087c-4290-82c5-582e3dff34ec"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3d1cc1a8-b027-46d9-abbd-ee618d1c72a1"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("92ade242-36c1-4a85-9a7f-26576e318b5c"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("9c9cee76-6495-4806-8d15-34ce7a4fe100"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("44a1c0a0-644b-4751-8a78-89ee043adb84"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("7b5d199e-7a5e-492a-a303-ae1fbfffdc3b"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("9639d831-4df1-4088-a36f-6e1a94cd35c5"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "starter_time",
                table: "work_shift",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_time",
                table: "work_shift",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0b355454-a2ee-4cab-b8dd-1838fef61b8a"), "update" },
                    { new Guid("101d686e-54ae-4628-a1dd-79d7dbda296f"), "list" },
                    { new Guid("2d09d882-7fe5-4bee-8083-db08d6caa38f"), "create" },
                    { new Guid("84c692c7-e1ee-4e3a-af91-bb7dc3d783aa"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("29fc6aac-1abd-401d-a6be-ba42dff17435"), "company" },
                    { new Guid("6e2437a6-4e55-4d0e-baa2-97063f7287cb"), "employee" },
                    { new Guid("8d9e390e-cd19-430c-a752-b38efe4bf611"), "work" }
                });
        }
    }
}
