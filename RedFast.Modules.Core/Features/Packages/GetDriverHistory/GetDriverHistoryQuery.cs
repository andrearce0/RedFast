using MediatR;

namespace RedFast.Modules.Core.Features.Packages.GetDriverHistory;

public record GetDriverHistoryQuery
(
    Guid UserId    
) : IRequest<List<DriverHistoryViewModel>>;
