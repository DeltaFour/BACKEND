using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DeltaFour.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class create_other_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_company_address_AddressId",
                table: "company");

            migrationBuilder.DropForeignKey(
                name: "FK_role_company_CompanyId",
                table: "role");

            migrationBuilder.DropTable(
                name: "user_auth");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "role",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "role",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "role",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "role",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "role",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "role",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "role",
                newName: "company_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_CompanyId",
                table: "role",
                newName: "IX_role_company_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "company",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Cnpj",
                table: "company",
                newName: "cnpj");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "company",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "company",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "company",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "company",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "company",
                newName: "address_id");

            migrationBuilder.RenameIndex(
                name: "IX_company_AddressId",
                table: "company",
                newName: "IX_company_address_id");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "address",
                newName: "street");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "address",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "address",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "District",
                table: "address",
                newName: "district");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "address",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "address",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "address",
                newName: "zip_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "address",
                newName: "created_at");

            migrationBuilder.AlterColumn<Guid>(
                name: "address_id",
                table: "company",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "action",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "company_geolocation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    coord = table.Column<Point>(type: "point", nullable: false),
                    radius_meters = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_geolocation", x => x.id);
                    table.ForeignKey(
                        name: "FK_company_geolocation_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cellphone = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_allowed_by_pass_coord = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_login = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employee_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "work_shift",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    shift_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    starter_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    tolerance_minutes = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_shift", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employee_attendance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    punch_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    punch_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    coord = table.Column<Point>(type: "point", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_attendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_attendance_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employee_auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    refresh_token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_auth", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_auth_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employee_face",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FaceTemplate = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_face", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_face_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    action_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permission_action_action_id",
                        column: x => x.action_id,
                        principalTable: "action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_location_location_id",
                        column: x => x.location_id,
                        principalTable: "location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "employee_shift",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    shift_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    employee_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    updated_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_shift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_shift_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employee_shift_work_shift_shift_id",
                        column: x => x.shift_id,
                        principalTable: "work_shift",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_company_geolocation_company_id",
                table: "company_geolocation",
                column: "company_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_company_id",
                table: "employee",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_role_id",
                table: "employee",
                column: "role_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_attendance_employee_id",
                table: "employee_attendance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_auth_employee_id",
                table: "employee_auth",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_face_employee_id",
                table: "employee_face",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_shift_employee_id",
                table: "employee_shift",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_shift_shift_id",
                table: "employee_shift",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_action_id",
                table: "role_permission",
                column: "action_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_location_id",
                table: "role_permission",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_role_id",
                table: "role_permission",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_company_address_address_id",
                table: "company",
                column: "address_id",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_company_company_id",
                table: "role",
                column: "company_id",
                principalTable: "company",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_company_address_address_id",
                table: "company");

            migrationBuilder.DropForeignKey(
                name: "FK_role_company_company_id",
                table: "role");

            migrationBuilder.DropTable(
                name: "company_geolocation");

            migrationBuilder.DropTable(
                name: "employee_attendance");

            migrationBuilder.DropTable(
                name: "employee_auth");

            migrationBuilder.DropTable(
                name: "employee_face");

            migrationBuilder.DropTable(
                name: "employee_shift");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "work_shift");

            migrationBuilder.DropTable(
                name: "action");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "role",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "role",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "role",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "role",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "role",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "role",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "role",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_role_company_id",
                table: "role",
                newName: "IX_role_CompanyId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "company",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "cnpj",
                table: "company",
                newName: "Cnpj");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "company",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "company",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "company",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "company",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "address_id",
                table: "company",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_company_address_id",
                table: "company",
                newName: "IX_company_AddressId");

            migrationBuilder.RenameColumn(
                name: "street",
                table: "address",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "address",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "address",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "district",
                table: "address",
                newName: "District");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "address",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "address",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "zip_code",
                table: "address",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "address",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddressId",
                table: "company",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompanyId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Cellphone = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAllowedBypassCoord = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_auth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_auth_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_user_CompanyId",
                table: "user",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_user_RoleId",
                table: "user",
                column: "RoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_auth_UserId",
                table: "user_auth",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_company_address_AddressId",
                table: "company",
                column: "AddressId",
                principalTable: "address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_company_CompanyId",
                table: "role",
                column: "CompanyId",
                principalTable: "company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
