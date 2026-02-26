using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relation_and_seeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "work_shift",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

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
                name: "IX_work_shift_company_id",
                table: "work_shift",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_work_shift_company_company_id",
                table: "work_shift",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_work_shift_company_company_id",
                table: "work_shift");

            migrationBuilder.DropIndex(
                name: "IX_work_shift_company_id",
                table: "work_shift");

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

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "work_shift");
        }
    }
}
