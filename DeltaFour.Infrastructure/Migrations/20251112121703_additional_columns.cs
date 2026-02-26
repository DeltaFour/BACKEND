using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class additional_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("3d112c4a-88dc-4c32-abfe-689658d98483"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("4049f1e3-580e-4330-8b34-c786954309a1"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("9c4ac70b-eb5c-4718-9da4-9d7ef6eeab34"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("bee6edd1-4ed8-4c95-aea5-4a94bcd45b9c"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("54a72b0d-81a6-45c6-8483-3c86f268cad4"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("9299bb9e-7059-4bd8-ab12-28a1d433e723"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("9508a2c0-877d-4422-884b-2fd0613b74b8"));

            migrationBuilder.AddColumn<int>(
                name: "shift_type",
                table: "employee_attendance",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "shift_type",
                table: "employee_attendance");

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("3d112c4a-88dc-4c32-abfe-689658d98483"), "list" },
                    { new Guid("4049f1e3-580e-4330-8b34-c786954309a1"), "create" },
                    { new Guid("9c4ac70b-eb5c-4718-9da4-9d7ef6eeab34"), "delete" },
                    { new Guid("bee6edd1-4ed8-4c95-aea5-4a94bcd45b9c"), "update" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("54a72b0d-81a6-45c6-8483-3c86f268cad4"), "employee" },
                    { new Guid("9299bb9e-7059-4bd8-ab12-28a1d433e723"), "work" },
                    { new Guid("9508a2c0-877d-4422-884b-2fd0613b74b8"), "company" }
                });
        }
    }
}
