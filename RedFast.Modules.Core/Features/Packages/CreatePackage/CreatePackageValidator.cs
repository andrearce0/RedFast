using FluentValidation;

namespace RedFast.Modules.Core.Features.Packages.CreatePackage;

public class CreatePackageValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageValidator()
    {
        RuleFor(p => p.OriginAddress)
            .NotEmpty().WithMessage("O endereço de origem não pode ser vazio")
            .MinimumLength(10).WithMessage("O endereço deve ter, ao menos, 10 caracteres");

        RuleFor(x => x.DestinationAddress)
            .NotEmpty().WithMessage("O endereço de destino não pode ser vazio.")
            .MinimumLength(10).WithMessage("O endereço deve ter, ao menos, 10 caracteres.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("O peso deve ser maior que zero.")
            .LessThanOrEqualTo(500).WithMessage("O peso não pode exceder 500 kg.");
    }
}
