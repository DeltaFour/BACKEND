using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DeltaFour.Application.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
