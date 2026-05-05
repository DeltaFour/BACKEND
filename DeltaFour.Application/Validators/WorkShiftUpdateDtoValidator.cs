using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class WorkShiftUpdateDtoValidator : AbstractValidator<WorkShiftUpdateDto>
{
    public WorkShiftUpdateDtoValidator()
    {
        Include(new WorkShiftDtoValidator());

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é obrigatório");
    }
}
