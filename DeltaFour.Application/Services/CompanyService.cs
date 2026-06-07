using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Application.Mappers;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Services;

public class CompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public CompanyService(IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task Create(CreateCompanyRequest request, Guid userId)
    {
        var company = new Company()
        {
            Name = request.Name,
            LegalName = request.Name,
            Cnpj = request.Cnpj,
            IsActive = true,
            CreatedBy = userId,
        };


        var role = new Role()
        {
            Name = "ADMIN",
            CompanyId = company.Id,
            IsActive = true,
            CreatedBy = userId,
        };

        var user = new User()
        {
            Name = request.Name,
            Email = request.User.Email,
            Password = _passwordService.Hash(request.User.Password!),
            CreatedBy = userId,
            CompanyId = company.Id,
            RoleId = role.Id,
            IsActive = true,
            IsConfirmed = true,
        };

        _unitOfWork.CompanyRepository.Create(company);
        _unitOfWork.RoleRepository.Create(role);
        _unitOfWork.UserRepository.Create(user);

        var matutino = new WorkShift
        {
            ShiftType = ShiftType.Matutino,
            StartTime = new TimeOnly(6, 0),
            EndTime = new TimeOnly(14, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = userId
        };

        var diurno = new WorkShift
        {
            ShiftType = ShiftType.Diurno,
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(22, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = userId
        };

        var noturno = new WorkShift
        {
            ShiftType = ShiftType.Noturno,
            StartTime = new TimeOnly(22, 0),
            EndTime = new TimeOnly(6, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = userId
        };

        _unitOfWork.WorkShiftRepository.CreateRange(new[] { matutino, diurno, noturno });

        await _unitOfWork.Save();
    }

    public async Task<CompanyGetSettingsDto?> Get(UserContext user)
    {
        var dto = await _unitOfWork.CompanyRepository.GetSettings(user.CompanyId);
        if (dto != null)
        {
            dto.Email = user.Email!;
        }

        return dto;
    }

    public async Task UpdateSettings(CompanyGetSettingsDto dto, UserContext user)
    {
        var company = await _unitOfWork.CompanyRepository.FindWithCoordinates(user.CompanyId);

        CompanyGeolocation? geolocation = await
            _unitOfWork.CompanyGeolocationRepository.Find(c => c.CompanyId == user.CompanyId);
        if (company != null)
        {
            CompanyMapper.UpdateCompany(company, dto);
            _unitOfWork.CompanyRepository.Update(company);
            if (company.AddressId != null)
            {
                AddressMapper.UpdateAddress(dto, company.Address!);
                _unitOfWork.AddressRepository.Update(company.Address!);
            }
            else
            {
                Address address = AddressMapper.MapToAddress(dto);
                _unitOfWork.AddressRepository.Create(address);
                company.AddressId = address.Id;
            }

            if (geolocation != null)
            {
                GeolocationMapper.UpdateGeolocation(dto, geolocation);
                _unitOfWork.CompanyGeolocationRepository.Update(geolocation);
            }
            else
            {
                _unitOfWork.CompanyGeolocationRepository.Create(
                    GeolocationMapper.MapToGeolocation(dto, user.CompanyId, user.Id));
            }

            await _unitOfWork.Save();
        }
    }

    public async Task<ListCompaniesResponse> List()
    {
        var companies = await _unitOfWork.CompanyRepository.FindAll(c => new GetCompaniesItemResponse
        {
            Id = c.Id,
            Name = c.Name,
            Cnpj = c.Cnpj,
            IsActive = c.IsActive,
        });

        return new ListCompaniesResponse
        {
            Data = companies,
        };
    }

    public async Task ChangeStatus(Guid companyId)
    {
        var company = await _unitOfWork.CompanyRepository.Find(c => c.Id == companyId);

        company!.IsActive = !company.IsActive;

        await _unitOfWork.Save();
    }
}
