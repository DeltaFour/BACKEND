using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relation_User_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "user",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_user_RoleId",
                table: "user",
                column: "RoleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_RoleId",
                table: "user",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_role_RoleId",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_RoleId",
                table: "user");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "user");
        }
    }
}
