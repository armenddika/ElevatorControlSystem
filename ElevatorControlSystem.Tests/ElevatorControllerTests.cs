using ElevatorControlSystem.Models;
using ElevatorControlSystem.Services;
using Xunit;

namespace ElevatorControlSystem.Tests;

public class ElevatorTests
{
    [Fact]
    public void Requests_In_Same_Direction_Should_Use_Same_Elevator()
    {
        var controller = new ElevatorController(2, 10);

        var request1 = new ElevatorRequest(4, 10); // going up
        controller.RequestElevator(request1);

        var assignedElevator = controller.Elevators.First(e => e.Stops.Contains(4));

        var request2 = new ElevatorRequest(5, 9); // also going up
        controller.RequestElevator(request2);

        Assert.True(assignedElevator.Stops.ToList().Contains(5),
            "Expected the same elevator to be assigned for both up requests.");
        var otherElevator = controller.Elevators.First(e => e != assignedElevator);
        Assert.False(otherElevator.Stops.ToList().Contains(5),
            "The other elevator should not be assigned to floor 5.");
    }

    [Fact]
    public void Request_Should_Fallback_To_LeastBusy_If_No_Match()
    {
        var controller = new ElevatorController(3, 10);

        controller.Elevators[0].AddPickupRequest(new ElevatorRequest(9, 1)); // down
        controller.Elevators[1].AddPickupRequest(new ElevatorRequest(8, 2)); // down
        controller.Elevators[2].AddPickupRequest(new ElevatorRequest(2, 7));
        controller.Elevators[2].AddPickupRequest(new ElevatorRequest(3, 8));

        foreach (var elevator in controller.Elevators)
            elevator.MoveOneStep(0, 0);

        var request = new ElevatorRequest(1, 10); // going up
        controller.RequestElevator(request);

        var expected = controller.Elevators[0];
        Assert.True(expected.Stops.ToList().Contains(1),
            "Expected elevator 0 to be assigned to floor 1 as fallback.");
    }
}