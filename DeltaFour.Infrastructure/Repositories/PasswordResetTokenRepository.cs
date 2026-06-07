using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DeltaFour.Infrastructure.Repositories;

public class PasswordResetTokenRepository(AppDbContext context) : IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> FindValid(Guid userId, string code)
    {
        return await context.PasswordResetTokens
            .Where(t => t.UserId == userId && t.Code == code && t.UsedAtUtc == null)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public void Create(PasswordResetToken token)
    {
        context.PasswordResetTokens.Add(token);
    }

    public void Update(PasswordResetToken token)
    {
        context.PasswordResetTokens.Update(token);
    }
}
