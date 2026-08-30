using FluentAssertions;
using RedFast.Modules.Core.Entities;
using RedFast.Modules.Core.Entities.Enums;

namespace RedFast.Modules.Core.Tests.Entities;

public class PackageTests
{
    [Fact]
    public void UpdateStatus_ToAwaitingPickup_FromCreated_ShouldUpdateStatusAndAddEvent()
    {
        var package = new Package
        {
            Id = Guid.NewGuid(),
            TrackingCode = "TST-123456",
            OriginAddress = "Rua A",
            DestinationAddress = "Rua B",
            Weight = 10.5m,
            SenderId = Guid.NewGuid()
        };

        package.UpdateStatus(PackageStatus.AwaitingPickup, "Aguardando motorista", null);

        package.CurrentStatus.Should().Be(PackageStatus.AwaitingPickup);
        package.Events.Should().HaveCount(1);
        package.Events.First().Status.Should().Be(PackageStatus.AwaitingPickup);
    }

    [Fact]
    public void UpdateStatus_ToDelivered_FromCreated_ShouldThrowException()
    {
        var package = new Package
        {
            Id = Guid.NewGuid(),
            TrackingCode = "TST-123456",
            OriginAddress = "Rua A",
            DestinationAddress = "Rua B",
            Weight = 10.5m,
            SenderId = Guid.NewGuid()
        };

        Action action = () => package.UpdateStatus(PackageStatus.Delivered, "Ação não permitida", null);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Transição de status inválida*");
    }
}
