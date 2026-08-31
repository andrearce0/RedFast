using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities.Outbox;
using RedFast.Modules.Core.Infrastructure.Messaging;
using RedFast.Modules.Core.Persistence;
using System.Text.Json;

namespace RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;

public class UpdatePackageStatusHandler : IRequestHandler<UpdatePackageStatusCommand, Unit>
{
    private readonly RedFastDbContext _context;

    public UpdatePackageStatusHandler(RedFastDbContext context)
    {
        _context = context;
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

        var statusEvent = new PackageStatusChangedEvent(
            package.Id,
            oldStatus,
            package.CurrentStatus.ToString(),
            DateTimeOffset.UtcNow
        );

        var outboxMessage = new OutboxMessage
        {
            EventType = statusEvent.GetType().AssemblyQualifiedName ?? statusEvent.GetType().Name,
            Content = JsonSerializer.Serialize(statusEvent)
        };

        _context.OutboxMessages.Add(outboxMessage);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
