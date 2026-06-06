using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class AddressMapper
    {
        public static Address MapToAddress(CompanyGetSettingsDto dto)
        {
            return new Address()
            {
                Street = dto.Rua,
                District = dto.Bairro,
                City = dto.Cidade,
                State = dto.Estado,
                ZipCode = dto.Cep,
                Complement = dto.Complemento
            };
        }
    }
}
