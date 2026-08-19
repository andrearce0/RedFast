using FluentValidation;

namespace RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;

public class UpdatePackageStatusValidator : AbstractValidator<UpdatePackageStatusCommand>
{
    public UpdatePackageStatusValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("O ID do pacote é obrigatório.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Status incorreto.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
