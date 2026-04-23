using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Nome do cargo é obrigatório");

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

        RuleFor(x => x.CellPhone)
            .NotEmpty()
            .WithMessage("Celular é obrigatório");

        RuleFor(x => x.UserShift)
            .NotEmpty()
            .WithMessage("Turnos do usuário são obrigatórios");

        RuleFor(x => x.ImageBase64)
            .NotEmpty()
            .WithMessage("Imagem é obrigatória");
    }
}
