using FluentValidation;
using DeltaFour.Application.Dtos;

namespace DeltaFour.Application.Validators;

public class UserShiftDtoValidator : AbstractValidator<UserShiftDto>
{
    public UserShiftDtoValidator()
    {
        RuleFor(x => x.ShiftId)
            .NotEmpty()
            .WithMessage("Id do turno é obrigatório");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Data de início é obrigatória");
    }
}
