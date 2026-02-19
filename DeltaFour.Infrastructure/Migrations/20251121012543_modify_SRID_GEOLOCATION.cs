using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modify_SRID_GEOLOCATION : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 119, DateTimeKind.Utc).AddTicks(7923),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 118, DateTimeKind.Utc).AddTicks(1248),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 115, DateTimeKind.Utc).AddTicks(8936),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0b9ce114-7445-4311-b721-dc8fc54ae64c"), "create" },
                    { new Guid("4d963213-ea6c-4062-924a-82cf051f1101"), "list" },
                    { new Guid("8d5a0bad-9fe6-4a5b-a2af-f3cfe8cf1b34"), "delete" },
                    { new Guid("f120cc17-a615-463b-8377-6ecd91d2aa5b"), "update" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("228230ff-8bd3-413a-be32-bb0a3b55ccfe"), "work" },
                    { new Guid("e24daba8-f87f-4c48-9688-2cdb78fe9c64"), "employee" },
                    { new Guid("ffcd3365-6110-46f4-8fa3-cc13cbcd55df"), "company" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("0b9ce114-7445-4311-b721-dc8fc54ae64c"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4d963213-ea6c-4062-924a-82cf051f1101"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("8d5a0bad-9fe6-4a5b-a2af-f3cfe8cf1b34"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("f120cc17-a615-463b-8377-6ecd91d2aa5b"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("228230ff-8bd3-413a-be32-bb0a3b55ccfe"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("e24daba8-f87f-4c48-9688-2cdb78fe9c64"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("ffcd3365-6110-46f4-8fa3-cc13cbcd55df"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 119, DateTimeKind.Utc).AddTicks(7923));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "employee",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 118, DateTimeKind.Utc).AddTicks(1248));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "company",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 21, 1, 25, 43, 115, DateTimeKind.Utc).AddTicks(8936));

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
    }
}
