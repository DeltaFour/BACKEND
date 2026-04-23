using FluentValidation;
using DeltaFour.Application.Dtos.Requests;

namespace DeltaFour.Application.Validators;

public class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.CompanyEmail)
            .NotEmpty()
            .WithMessage("Email da empresa é obrigatório")
            .EmailAddress()
            .WithMessage("Email da empresa inválido");
    }
}
