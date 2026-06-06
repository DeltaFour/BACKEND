namespace DeltaFour.Application.Dtos.Responses.Company
{
    public class CompanyGetSettingsDto
    {
        public String? RazaoSocial { get; set; }
        
        public String? NomeFantasia { get; set; }
        
        public String Cnpj { get; set; }
        
        public String Email { get; set; }
        
        public String? Cep { get; set; }
        
        public String? Complemento { get; set; }
        
        public String? Rua { get; set; }
        
        public String? Numero { get; set; }
        
        public String? Bairro { get; set; }
        
        public String? Cidade { get; set; }
        
        public String? Estado { get; set; }

        public Double? Latitude { get; set; } = 0;

        public Double? Longitude { get; set; } = 0;
        
        public int? RaioMetros { get; set; }
    }
}
