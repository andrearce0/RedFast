using FluentValidation;

namespace RedFast.Modules.Core.Features.Auth.LoginUser;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(u => u.Email).NotEmpty();

        RuleFor(u => u.Password).NotEmpty();
    }
}
