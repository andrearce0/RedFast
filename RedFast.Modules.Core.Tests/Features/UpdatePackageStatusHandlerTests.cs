using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RedFast.Modules.Core.Entities;
using RedFast.Modules.Core.Entities.Enums;
using RedFast.Modules.Core.Features.Packages.UpdatePackageStatus;
using RedFast.Modules.Core.Infrastructure.Messaging;
using RedFast.Modules.Core.Persistence;

namespace RedFast.Modules.Core.Tests.Features;

public class UpdatePackageStatusHandlerTests
{
    private readonly RedFastDbContext _context;
    private readonly IMessageBus _messageBusMock;
    private readonly UpdatePackageStatusHandler _handler;

    public UpdatePackageStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<RedFastDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RedFastDbContext(options);

        _messageBusMock = Substitute.For<IMessageBus>();

        _handler = new UpdatePackageStatusHandler(_context, _messageBusMock);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateStatusAndPublishMessage()
    {
        var packageId = Guid.NewGuid();
        var package = new Package
        {
            Id = packageId,
            TrackingCode = "TST-001",
            OriginAddress = "Rua A",
            DestinationAddress = "Rua B",
            Weight = 5m,
            SenderId = Guid.NewGuid()
        };

        package.UpdateStatus(PackageStatus.AwaitingPickup, "Aguardando", null);

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        var command = new UpdatePackageStatusCommand(
                UserId: Guid.NewGuid(),
                UserRole: "admin",
                PackageId: packageId,
                NewStatus: PackageStatus.Collected,
                Description: "Coletado pelo motorista",
                Location: "Centro"
        );

        await _handler.Handle(command, CancellationToken.None);

        var updatedPackage = await _context.Packages.FindAsync(packageId);
        updatedPackage.Should().NotBeNull();
        updatedPackage!.CurrentStatus.Should().Be(PackageStatus.Collected);

        await _messageBusMock.Received(1).PublishAsync(
            Arg.Any<PackageStatusChangedEvent>(),
            "package.status.changed");
    }

    [Fact]
    public async Task Handle_PackageNotFound_ShouldThrowExceptionAndNotPublishMessage()
    {
        var command = new UpdatePackageStatusCommand
        (
            UserId: Guid.NewGuid(),
            UserRole: "admin",
            PackageId: Guid.NewGuid(),
            NewStatus: PackageStatus.AwaitingPickup,
            Description: "",
            Location: ""
        );

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*não encontrado*");

        await _messageBusMock.DidNotReceive().PublishAsync(
            Arg.Any<PackageStatusChangedEvent>(),
            Arg.Any<string>()
        );
    }
}
