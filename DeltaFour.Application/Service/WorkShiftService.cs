using DeltaFour.Application.Dtos;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace DeltaFour.Application.Service
{
    public class WorkShiftService(AllRepositories allRepositories)
    {
        ///<sumary>
        ///Operation for get all WorkShift from company
        ///</sumary>
        public async Task<List<WorkShiftResponseDto>?> Get(Guid companyId)
        {
            List<WorkShiftResponseDto> workShifts =
                await allRepositories.WorkShiftRepository.FindAll(ws => ws.CompanyId == companyId);
            if (workShifts.Count > 0)
            {
                return workShifts;
            }

            return null;
        }
        
        ///<sumary>
        ///Operation for create WorkShift
        ///</sumary>
        public async Task Create(WorkShiftDto dto, UserContext user)
        {
            WorkShift workShift = WorkShiftMapper.CreateFromDto(dto, user);
            allRepositories.WorkShiftRepository.Create(workShift);
            await allRepositories.Save();
        }

        ///<sumary>
        ///Operation for update WorkShift
        ///</sumary>
        public async Task Update(WorkShiftUpdateDto dto, UserContext user)
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

        ///<sumary>
        ///Operation for change status from WorkShift 
        ///</sumary>
        public async Task Delete(Guid workShiftId, Guid companyId)
        {
            if (!await allRepositories.UserShiftRepository.FindAny(es => es.ShiftId == workShiftId))
            {
                WorkShift workShift =
                    (await allRepositories.WorkShiftRepository.Find(ws =>
                        ws.Id == workShiftId && ws.CompanyId == companyId))!;
                allRepositories.WorkShiftRepository.Delete(workShift);
                await allRepositories.Save();
                return;
            }

            throw new BadHttpRequestException(
                "Não foi possível deletar este horario, ainda há usuários registrado com ele");
        }
    }
}
