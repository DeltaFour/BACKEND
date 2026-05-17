using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    public partial class add_system_company_and_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var companyId = new Guid("11111111-1111-4111-8111-111111111111");
            var createdBy = new Guid("22222222-2222-4222-8222-222222222222");

            var roleRhId = new Guid("33333333-3333-4333-8333-333333333333");
            var roleEmployeeId = new Guid("44444444-4444-4444-8444-444444444444");
            var roleAdminId = new Guid("55555555-5555-4555-8555-555555555555");

            // Insert default system company
            migrationBuilder.InsertData(
                table: "company",
                columns: new[] { "id", "name", "cnpj", "is_active", "created_by" },
                values: new object[] { companyId, "System", "00000000000000", true, createdBy }
            );

            // Insert default roles linked to system company
            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "Id", "name", "is_active", "company_id", "created_by" },
                values: new object[,]
                {
                    { roleRhId, "RH", true, companyId, createdBy },
                    { roleEmployeeId, "EMPLOYEE", true, companyId, createdBy },
                    { roleAdminId, "ADMIN", true, companyId, createdBy }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var roleRhId = new Guid("33333333-3333-4333-8333-333333333333");
            var roleEmployeeId = new Guid("44444444-4444-4444-8444-444444444444");
            var roleAdminId = new Guid("55555555-5555-4555-8555-555555555555");

            var companyId = new Guid("11111111-1111-4111-8111-111111111111");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "Id",
                keyValue: roleRhId
            );

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "Id",
                keyValue: roleEmployeeId
            );

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "Id",
                keyValue: roleAdminId
            );

            migrationBuilder.DeleteData(
                table: "company",
                keyColumn: "id",
                keyValue: companyId
            );
        }
    }
}
