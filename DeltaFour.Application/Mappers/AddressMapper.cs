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
                Number = dto.Numero,
                Complement = dto.Complemento
            };
        }

        public static void UpdateAddress(CompanyGetSettingsDto dto, Address address)
        {
            address.Street = dto.Rua;
            address.District = dto.Bairro;
            address.City = dto.Cidade;
            address.State = dto.Estado;
            address.ZipCode = dto.Cep;
            address.Number = dto.Numero;
            address.Complement = dto.Complemento;
        } 
    }
}
