using DeltaFour.Application.Service;
using DeltaFour.Application.Utils;
using DeltaFour.Application.Service.Company;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Infrastructure.Repositories;
using DeltaFour.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CreateCompanyService = DeltaFour.Application.Service.Company.CreateService;
using ListCompanyService = DeltaFour.Application.Service.Company.ListService;

namespace DeltaFour.CrossCutting.Ioc;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var isTesting = Environment.GetEnvironmentVariable("IS_TESTING")!;

        if (isTesting.ToLower().Equals("true"))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        }
        else
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    uoptions => uoptions.UseNetTopologySuite()));
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeAuthRepository, EmployeeAuthRepository>();
            services.AddScoped<AuthService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<CompanyService>();
            services.AddScoped<WorkShiftService>();
            services.AddScoped<AllRepositories>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IActionRepository, ActionRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();
            services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
            services.AddScoped<IEmployeeShiftRepository, EmployeeShiftRepository>();
            services.AddScoped<ICompanyGeolocationRepository, CompanyGeolocationRepository>();
            services.AddScoped<IEmployeeFaceRepository, EmployeeFaceRepository>();
            services.AddScoped<IUnitOfWork, AllRepositories>();
            services.AddScoped<PythonExe>();
            services.AddScoped<SuperAdminSeeder>();
            services.AddScoped<StatusService>();
            services.AddScoped<CreateCompanyService>();
            services.AddScoped<ListCompanyService>();
        }
        return services;
    }
}