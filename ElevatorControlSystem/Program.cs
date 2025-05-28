using ElevatorControlSystem.Models;
using ElevatorControlSystem.Services;

var controller = new ElevatorController(4, totalFloors: 10);
bool exit = false;

Console.WriteLine("Elevator simulation started. Press 'Q' to quit or 'P' to process predefined requests.");

while (!exit)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(intercept: true).Key;

        if (key == ConsoleKey.Q)
        {
            exit = true;
            continue;
        }
        else if (key == ConsoleKey.P)
        {
            Console.WriteLine("Processing predefined requests...");
            var predefinedRequests = new[]
            {
                new ElevatorRequest(3, 7), // Request to go up from floor 3
                new ElevatorRequest(2, 6), // Request to go up from floor 2
                new ElevatorRequest(5, 1)  // Request to go down from floor 5
            };

            controller.RequestElevators(predefinedRequests);
            controller.StepAll();
            controller.PrintStatus();
            Console.WriteLine(new string('-', 40));
            continue;
        }
    }

    // Random request generation logic
    var random = new Random();
    if (random.NextDouble() < 0.3)
    {
        int fromFloor = random.Next(1, 11);
        int toFloor;

        do
        {
            toFloor = random.Next(1, 11);
        } while (toFloor == fromFloor);

        var request = new ElevatorRequest(fromFloor, toFloor);
        controller.RequestElevator(request);
    }

    controller.StepAll();
    controller.PrintStatus();
    Console.WriteLine(new string('-', 40));

    Thread.Sleep(1000);
}

Console.WriteLine("Simulation stopped.");