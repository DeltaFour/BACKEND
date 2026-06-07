namespace DeltaFour.Application.Dtos.Responses.Company
{
    public class CompanyGetSettingsDto
    {
        public string? RazaoSocial { get; set; }

        public string? NomeFantasia { get; set; }

        public string Cnpj { get; set; }

        public string Email { get; set; }

        public string? Cep { get; set; }

        public string? Complemento { get; set; }

        public string? Rua { get; set; }

        public int? Numero { get; set; }

        public string? Bairro { get; set; }

        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public double? Latitude { get; set; } = 0;

        public double? Longitude { get; set; } = 0;

        public int? RaioMetros { get; set; } = 100;
    }
}
