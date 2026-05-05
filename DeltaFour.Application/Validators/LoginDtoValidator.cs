using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Senha deve ter no mínimo 8 caracteres")
            .MaximumLength(32)
            .WithMessage("Senha deve ter no máximo 32 caracteres");
    }
}
