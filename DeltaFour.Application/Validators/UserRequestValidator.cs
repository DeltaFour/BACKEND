using FluentValidation;
using DeltaFour.Application.Dtos.Requests;

namespace DeltaFour.Application.Validators;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email inválido");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MinimumLength(3)
            .WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(128)
            .WithMessage("Nome deve ter no máximo 128 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("Senha deve ter no mínimo 8 caracteres")
            .MaximumLength(32)
            .WithMessage("Senha deve ter no máximo 32 caracteres");
    }
}
