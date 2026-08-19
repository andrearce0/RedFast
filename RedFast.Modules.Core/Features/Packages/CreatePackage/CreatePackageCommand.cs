using MediatR;

namespace RedFast.Modules.Core.Features.Packages.CreatePackage;

public record CreatePackageCommand
(
    Guid UserId,
    string OriginAddress,
    string DestinationAddress,
    decimal Weight) : IRequest<Guid>;   