using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class WorkShiftDtoValidator : AbstractValidator<WorkShiftDto>
{
    public WorkShiftDtoValidator()
    {
        RuleFor(x => x.ShiftType)
            .IsInEnum()
            .WithMessage("Tipo de turno inválido");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Horário de início é obrigatório");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("Horário de término é obrigatório");

        RuleFor(x => x.ToleranceMinutes)
            .NotEmpty()
            .WithMessage("Minutos de tolerância são obrigatórios");
    }
}
