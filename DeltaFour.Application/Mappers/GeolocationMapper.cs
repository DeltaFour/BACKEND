using DeltaFour.Application.Dtos.Responses.Company;
using DeltaFour.Domain.Entities;

namespace DeltaFour.Application.Mappers
{
    public static class GeolocationMapper
    {
        public static CompanyGeolocation MapToGeolocation(CompanyGetSettingsDto dto, Guid companyId, Guid userId)
        {
            return new CompanyGeolocation()
            {
                CompanyId = companyId,
                Coord = new Coordinates(dto.Latitude.GetValueOrDefault(0), dto.Longitude.GetValueOrDefault(0)),
                RadiusMeters = dto.RaioMetros.GetValueOrDefault(0),
                IsActive = true,
                CreatedBy = userId,
            };
        }

        public static void UpdateGeolocation(CompanyGetSettingsDto dto, CompanyGeolocation geo)
        {
            geo.Coord = new Coordinates(dto.Latitude.GetValueOrDefault(0), dto.Longitude.GetValueOrDefault(0));
            geo.RadiusMeters = dto.RaioMetros.GetValueOrDefault(0);
            geo.IsActive = true;
            
        }
    }
}
