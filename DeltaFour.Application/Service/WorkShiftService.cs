using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace DeltaFour.Application.Service
{
    public class WorkShiftService(AllRepositories allRepositories)
    {
        public async Task Create(WorkShiftDto dto, Employee user)
        {
            WorkShift workShift = WorkShiftMapper.CreateFromDto(dto, user);
            allRepositories.WorkShiftRepository.Create(workShift);
            await allRepositories.Save();
        }

        public async Task Update(WorkShiftUpdateDto dto, Employee user)
        {
            WorkShift? workShift = await allRepositories.WorkShiftRepository.Find(ws => ws.Id == dto.Id);
            if (workShift != null)
            {
                WorkShiftMapper.UpdateDataWorkShift(workShift, dto, user);
                allRepositories.WorkShiftRepository.Update(workShift);
                await allRepositories.Save();
                return;
            }

            throw new InvalidOperationException("Erro interno!");
        }
    }
}
