using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserAttendanceRepository(AppDbContext context) : IUserAttendanceRepository
    {
        public async Task<UserAttendance?> Find(Expression<Func<UserAttendance, bool>> predicate)
        {
            return await context.EmployeeAttendances.FirstOrDefaultAsync(predicate);
        }
        public void Create(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Add(userAttendance);
        }
        public void Update(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Update(userAttendance);
        }
        public void Delete(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Remove(userAttendance);
        }
        public async Task UpdateStatusAttendance(Guid attendanceId)
        {
            var attendance = await context.EmployeeAttendances
                .FirstOrDefaultAsync(at => at.Id == attendanceId);

            attendance!.Status = Enum.GetName(StatusAttendance.aprovado);

            await context.SaveChangesAsync();
        }

        public async Task<int> AmountAttendanceIn(Guid userId)
        {
            var startOfDay = DateTime.Today;
            var endOfDay = startOfDay.AddHours(23).AddMinutes(59).AddSeconds(59);

            return await context.EmployeeAttendances
                .CountAsync(at =>
                    at.UserId == userId &&
                    at.PunchType == PunchType.IN &&
                    at.PunchTime >= startOfDay &&
                    at.PunchTime < endOfDay
                );
        }
        public async Task<int> AmountAttendanceInYesterday(Guid userId)
        {
            var startOfDay = DateTime.Today.AddSeconds(-1);
            var endOfDay = startOfDay.AddHours(-23).AddMinutes(-59).AddSeconds(-59);

            return await context.EmployeeAttendances
                .CountAsync(at =>
                    at.UserId == userId &&
                    at.PunchType == PunchType.IN &&
                    at.PunchTime >= startOfDay &&
                    at.PunchTime < endOfDay
                );
        }
    }
}