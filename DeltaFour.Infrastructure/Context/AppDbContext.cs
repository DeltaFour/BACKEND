using Microsoft.EntityFrameworkCore;

namespace DeltaFour.Infrastructure.Context;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
