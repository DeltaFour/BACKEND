using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_company_address_address_id",
                table: "company");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "company",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_by",
                table: "company",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_company_address_address_id",
                table: "company",
                column: "address_id",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_company_address_address_id",
                table: "company");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "company");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "company");

            migrationBuilder.AddForeignKey(
                name: "FK_company_address_address_id",
                table: "company",
                column: "address_id",
                principalTable: "address",
                principalColumn: "id");
        }
    }
}
