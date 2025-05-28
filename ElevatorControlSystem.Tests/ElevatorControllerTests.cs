using ElevatorControlSystem.Models;
using ElevatorControlSystem.Services;
using Xunit;

namespace ElevatorControlSystem.Tests;

public class ElevatorControllerTests
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
    public void Requests_In_Opposite_Directions_Should_Use_Different_Elevators()
    {
        var controller = new ElevatorController(2, 10);

        var request1 = new ElevatorRequest(4, 10); // going up
        controller.RequestElevator(request1);

        var request2 = new ElevatorRequest(6, 2); // going down
        controller.RequestElevator(request2);

        var upElevator = controller.Elevators.First(e => e.Stops.Contains(4));
        var downElevator = controller.Elevators.First(e => e.Stops.Contains(6));

        Assert.NotEqual(upElevator.Id, downElevator.Id);
    }

    [Fact]
    public void Idle_Elevator_Should_Be_Assigned_When_No_Matching_Direction()
    {
        var controller = new ElevatorController(3, 10); // Ensure there are enough elevators

        // Simulate busy elevators
        controller.Elevators[0].AddPickupRequest(new ElevatorRequest(5, 1)); // down
        controller.Elevators[1].AddPickupRequest(new ElevatorRequest(6, 2)); // down

        controller.Elevators[0].Direction = Direction.Down;
        controller.Elevators[1].Direction = Direction.Down;

        var beforeAssigningIdleElevator = controller.Elevators.FirstOrDefault(e => e.Direction == Direction.Idle);

        Assert.NotNull(beforeAssigningIdleElevator);
        // Leave one elevator idle
        var request = new ElevatorRequest(1, 10); // going up
        controller.RequestElevator(request);

        // Check if any idle elevator exists
        var afterAssigningRequestToIdleElevator =
            controller.Elevators.FirstOrDefault(e => e.Direction == Direction.Idle);
        Assert.Null(afterAssigningRequestToIdleElevator); // Ensure there is none idle elevator anymore
    }

    [Fact]
    public void Least_Busy_Elevator_Should_Be_Assigned_When_All_Are_Busy()
    {
        var controller = new ElevatorController(3, 10);

        controller.Elevators[0].AddPickupRequest(new ElevatorRequest(9, 1)); // down
        controller.Elevators[1].AddPickupRequest(new ElevatorRequest(8, 2)); // down
        controller.Elevators[2].AddPickupRequest(new ElevatorRequest(2, 7));
        controller.Elevators[2].AddPickupRequest(new ElevatorRequest(3, 5)); // up

        var request = new ElevatorRequest(1, 10); // going up
        controller.RequestElevator(request);

        var leastBusyElevatorStop = controller.Elevators.OrderBy(c => c.Stops.Count).First();
        Assert.Equal(1, leastBusyElevatorStop.Stops.Count); //should have only one stop
    }

    [Fact]
    public void Multiple_Requests_From_Same_Floor_Should_Be_Handled()
    {
        var controller = new ElevatorController(2, 10);

        var request1 = new ElevatorRequest(3, 7); // going up
        var request2 = new ElevatorRequest(3, 5); // going up

        controller.RequestElevator(request1);
        controller.RequestElevator(request2);

        var assignedElevator = controller.Elevators.First(e => e.Stops.Contains(3));
        Assert.Contains(3, assignedElevator.Stops);
    }

    [Fact]
    public void Elevator_Stops_Should_Be_Updated_After_Servicing_Request()
    {
        var controller = new ElevatorController(1, 10);

        var request = new ElevatorRequest(3, 7); // going up
        controller.RequestElevator(request);

        var elevator = controller.Elevators.First();
        Assert.Contains(3, elevator.Stops);

        elevator.MoveOneStep(); // Move to floor 2
        elevator.MoveOneStep(); // Move to floor 3 (pickup)

        Assert.DoesNotContain(3, elevator.Stops); // Floor 3 should be removed
        Assert.Contains(7, elevator.Stops); // Floor 7 should remain
    }
}