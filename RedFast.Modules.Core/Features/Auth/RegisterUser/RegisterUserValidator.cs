using FluentValidation;

namespace RedFast.Modules.Core.Features.Auth.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");
    
        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter, no mínimo, 8 caracteres");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Nome é obrigatório");

        RuleFor(u => u.Document)
            .NotEmpty().WithMessage("Documento é obrigatório");
    }
}
