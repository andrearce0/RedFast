using MediatR;
using Microsoft.EntityFrameworkCore;
using RedFast.Modules.Core.Entities.Enums;
using RedFast.Modules.Core.Persistence;

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
            throw new Exception("Pacote não encontrado.");
        
        if (request.UserRole == "driver")
        {
            var designatedDriver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == request.UserId, cancellationToken);
            if(designatedDriver == null)
                throw new InvalidOperationException("Não há motorista vinculado a esse perfil.");

            if (designatedDriver.Id != package.DriverId)
                throw new UnauthorizedAccessException("Acesso negado: você não tem autorização para alterar este pacote");
        }

        if(package.CurrentStatus == PackageStatus.Delivered ||
            package.CurrentStatus == PackageStatus.DeliveryFailed)
            throw new InvalidOperationException("Não é possível atualizar o status de um pacote que já concluído.");

        package.UpdateStatus(request.NewStatus, request.Description, request.Location);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
