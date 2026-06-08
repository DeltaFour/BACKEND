using Microsoft.AspNetCore.Http;

namespace DeltaFour.Application.Dtos
{
    public class PunchByEmailDto : PunchDto
    {
        public string? Justification { get; set; }

        /// <summary>
        /// Arquivo (imagem ou PDF) enviado como anexo do ponto, via multipart/form-data.
        /// </summary>
        public IFormFile? File { get; set; }

        public string? Observation { get; set; }
    }
}
