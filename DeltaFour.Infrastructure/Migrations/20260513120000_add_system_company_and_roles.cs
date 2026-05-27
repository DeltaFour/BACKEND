using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_system_company_and_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert default system company
            migrationBuilder.Sql(@"
                INSERT INTO company (id, name, cnpj, is_active, created_at, created_by)
                VALUES ('11111111-1111-4111-8111-111111111111', 'System', '00000000000000', 1, UTC_TIMESTAMP(), '22222222-2222-4222-8222-222222222222');
            ");

            // Insert default roles linked to system company
            migrationBuilder.Sql(@"
                INSERT INTO role (Id, company_id, name, is_active, created_by, created_at)
                VALUES 
                    ('33333333-3333-4333-8333-333333333333', '11111111-1111-4111-8111-111111111111', 'RH', 1, '22222222-2222-4222-8222-222222222222', UTC_TIMESTAMP()),
                    ('44444444-4444-4444-8444-444444444444', '11111111-1111-4111-8111-111111111111', 'EMPLOYEE', 1, '22222222-2222-4222-8222-222222222222', UTC_TIMESTAMP()),
                    ('55555555-5555-4555-8555-555555555555', '11111111-1111-4111-8111-111111111111', 'ADMIN', 1, '22222222-2222-4222-8222-222222222222', UTC_TIMESTAMP());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM role WHERE Id IN (
                    '33333333-3333-4333-8333-333333333333',
                    '44444444-4444-4444-8444-444444444444',
                    '55555555-5555-4555-8555-555555555555'
                );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM company WHERE id = '11111111-1111-4111-8111-111111111111';
            ");
        }
    }
}
