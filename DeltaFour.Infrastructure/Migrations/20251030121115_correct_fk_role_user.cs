using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class correct_fk_role_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_role_role_id",
                table: "employee");
            migrationBuilder.DropIndex(
                name: "IX_employee_role_id",
                table: "employee");

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("015738bc-67ed-4afc-9e9a-4b40c95f85e0"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("04e70d7d-8c9c-498e-bb5e-5cb2ef9c826d"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("85ed664f-31e1-4a4d-aa20-854908fc8c86"));

            migrationBuilder.DeleteData(
                table: "action",
                keyColumn: "id",
                keyValue: new Guid("ba176dff-5888-4988-86c2-95a32854d159"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("52644ef8-64fd-47d8-88d4-c8c802f56b4e"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("60e224a7-6dcb-49c6-b4ac-3b887857a07d"));

            migrationBuilder.DeleteData(
                table: "location",
                keyColumn: "id",
                keyValue: new Guid("ecb6bb33-bd54-43dc-9579-4a06a42207a4"));

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

            migrationBuilder.CreateIndex(
                name: "IX_employee_role_id",
                table: "employee",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employee_role_id",
                table: "employee");

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

            migrationBuilder.InsertData(
                table: "action",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("015738bc-67ed-4afc-9e9a-4b40c95f85e0"), "create" },
                    { new Guid("04e70d7d-8c9c-498e-bb5e-5cb2ef9c826d"), "list" },
                    { new Guid("85ed664f-31e1-4a4d-aa20-854908fc8c86"), "update" },
                    { new Guid("ba176dff-5888-4988-86c2-95a32854d159"), "delete" }
                });

            migrationBuilder.InsertData(
                table: "location",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("52644ef8-64fd-47d8-88d4-c8c802f56b4e"), "work" },
                    { new Guid("60e224a7-6dcb-49c6-b4ac-3b887857a07d"), "company" },
                    { new Guid("ecb6bb33-bd54-43dc-9579-4a06a42207a4"), "employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_role_id",
                table: "employee",
                column: "role_id",
                unique: true);
        }
    }
}
