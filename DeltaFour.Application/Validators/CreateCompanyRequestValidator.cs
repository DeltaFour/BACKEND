using FluentValidation;
using DeltaFour.Application.Dtos.Requests;

namespace DeltaFour.Application.Validators;

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome da empresa é obrigatório");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório");

        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("Dados do usuário são obrigatórios")
            .SetValidator(new UserRequestValidator());
    }
}
