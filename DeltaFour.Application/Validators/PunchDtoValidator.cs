using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class PunchDtoValidator : AbstractValidator<PunchDto>
{
    public PunchDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo de ponto inválido");

        RuleFor(x => x.TimePunched)
            .NotEmpty()
            .WithMessage("Horário do ponto é obrigatório");

        RuleFor(x => x.ImageBase64)
            .NotEmpty()
            .WithMessage("Imagem é obrigatória");

        RuleFor(x => x.ShiftType)
            .IsInEnum()
            .WithMessage("Tipo de turno inválido");
    }
}
