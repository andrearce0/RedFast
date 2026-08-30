using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities.Enums;
using RedFast.Modules.Core.Infrastructure.Messaging;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;

public class UpdatePackageStatusHandler : IRequestHandler<UpdatePackageStatusCommand, Unit>
{
    private readonly RedFastDbContext _context;
    private readonly IMessageBus _messageBus;

    public UpdatePackageStatusHandler(RedFastDbContext context,
        IMessageBus messageBus)
    {
        _context = context;
        _messageBus = messageBus;
    }

    public async Task<Unit> Handle(UpdatePackageStatusCommand request, CancellationToken cancellationToken)
    {
        var package = await _context.Packages.Include(p => p.Events).FirstOrDefaultAsync(p => p.Id == request.PackageId, cancellationToken);

        if (package == null)
            throw new InvalidOperationException("Pacote não encontrado.");
        
        if (request.UserRole == "driver")
        {
            var designatedDriver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == request.UserId, cancellationToken);
            if(designatedDriver == null)
                throw new InvalidOperationException("Não há motorista vinculado a esse perfil.");

            if (designatedDriver.Id != package.DriverId)
                throw new UnauthorizedAccessException("Acesso negado: você não tem autorização para alterar este pacote");
        }

        var oldStatus = package.CurrentStatus.ToString();

        package.UpdateStatus(request.NewStatus, request.Description, request.Location);

        await _context.SaveChangesAsync(cancellationToken);

        var statusEvent = new PackageStatusChangedEvent(
            package.Id,
            oldStatus,
            package.CurrentStatus.ToString(),
            DateTimeOffset.UtcNow
        );

        await _messageBus.PublishAsync(statusEvent, "package.status.changed");

        return Unit.Value;
    }
}
