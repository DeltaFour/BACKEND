using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class WorkShiftRepository : BaseRepository<WorkShift>, IWorkShiftRepository
    {
        public WorkShiftRepository(AppDbContext context) : base(context)
        {
        }

        public void Create(WorkShift workShift)
        {
            _context.WorkShifts.Add(workShift);
        }

        public void CreateRange(IEnumerable<WorkShift> workShifts)
        {
            _context.WorkShifts.AddRange(workShifts);
        }

        public void Update(WorkShift workShift)
        {
            _context.WorkShifts.Update(workShift);
        }

        public void Delete(WorkShift workShift)
        {
            _context.WorkShifts.Remove(workShift);
        }

        public async Task<List<WorkShiftResponseDto>> FindAll(Expression<Func<WorkShift, bool>> predicate)
        {
            return await _context.WorkShifts.Where(predicate)
                .Select(ws => new WorkShiftResponseDto()
                {
                    Id = ws.Id,
                    ShiftType = ws.ShiftType,
                    StartTime = ws.StartTime,
                    EndTime = ws.EndTime,
                    ToleranceMinutes = ws.ToleranceMinutes
                })
                .ToListAsync();
        }

        public async Task<WorkShiftPunchDto?> GetByUserIdAndIsActive(Guid userId, Guid companyId)
        {
            var query = from ws in _context.WorkShifts
                        join es in _context.EmployeeShifts on ws.Id equals es.ShiftId
                        where es.IsActive == true &&
                              es.UserId == userId
                              && ws.CompanyId == companyId
                        select new WorkShiftPunchDto()
                        {
                            StartTime = ws.StartTime,
                            EndTime = ws.EndTime,
                            ToleranceMinutes = ws.ToleranceMinutes
                        };
            return await query.FirstOrDefaultAsync();
        }
    }
}
