using ElevatorControlSystem.Models;
using ElevatorControlSystem.Services;

var controller = new ElevatorController(4, totalFloors: 10);
var random = new Random();
bool exit = false;

Console.WriteLine("Elevator simulation started. Press 'Q' to quit.");

while (!exit)
{
    if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
    {
        exit = true;
        continue;
    }

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