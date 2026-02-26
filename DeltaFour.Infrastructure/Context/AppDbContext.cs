using DeltaFour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Action = DeltaFour.Domain.Entities.Action;

namespace DeltaFour.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Employees { get; set; }

    public DbSet<Company> Companies { get; set; }
    
    public DbSet<Address> Addresses { get; set; }
    
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<UserAuth> Auth { get; set; }
    
    public DbSet<RolePermission> RolePermissions { get; set; }
    
    public DbSet<Action> Actions { get; set; }
    
    public DbSet<Location> Locations { get; set; }
    
    public DbSet<CompanyGeolocation> CompanyGeolocations { get; set; }
    
    public DbSet<UserAttendance> EmployeeAttendances { get; set; }
    
    public DbSet<UserFace> EmployeeFaces { get; set; }
    
    public DbSet<UserShift> EmployeeShifts { get; set; }
    
    public DbSet<WorkShift> WorkShifts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}