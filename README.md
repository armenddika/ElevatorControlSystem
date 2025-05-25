# Elevator Control System (.NET 8)

This project is a simulation of a basic elevator control system implemented in C#. It models a 10-floor building with 4 elevators and provides a console-based simulation of elevator movement and passenger boarding/debarking.

## Features

* 10-floor building
* 4 elevators
* Each elevator takes:

  * 10 seconds to move between floors
  * 10 seconds to load/unload passengers
* Elevators only change direction after finishing all stops in their current direction
* Passenger requests include both pickup and destination floors
* Destination floors are added only after the passenger boards
* Random elevator requests generated every second with a 30% chance
* Elevator requests are assigned based on direction, current position, and load

## Technologies

* .NET 8 Console Application
* C# with object-oriented principles
* xUnit for unit testing

## How to Run

1. Ensure you have [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.
2. Open a terminal in the project folder.
3. Run the application:

   ```bash
   dotnet run
   ```
4. Observe the simulation in the console.
5. Press `Q` to stop the simulation.

## How to Test

Unit tests cover elevator movement, direction handling, and request assignment logic.
To run the tests:

```bash
cd ElevatorControlSystem.Tests
 dotnet test
```

## Project Structure

```
ElevatorControlSystem/
├── Program.cs
├── Models/
│   ├── Elevator.cs
│   ├── ElevatorRequest.cs
│   └── Direction.cs
├── Services/
│   └── ElevatorController.cs
└── ElevatorControlSystem.csproj

ElevatorControlSystem.Tests/
├── ElevatorTests.cs
├── ElevatorControllerTests.cs
└── ElevatorControlSystem.Tests.csproj
```

## System Behavior Flow

1. The app starts and initializes 4 elevators.
2. Every second, it has a 30% chance to generate a random request.
3. A request consists of:
   * A pickup floor
   * A destination floor
4. The controller assigns the best elevator using the following order:
   * Elevators already moving in the right direction and passing by the pickup floor
   * Idle elevators
   * Least loaded elevators
5. The elevator picks up the passenger and only then adds the destination.
6. Console displays actions like:
   * Request assigned
   * Elevator boarding/debarking
   * Current status of all elevators

## Example Output

```
Request: Passenger at floor 6 → destination 2 assigned to Elevator 2
Elevator 2 arrived at floor 6 — boarding/unboarding for 10s
Passenger boarded elevator 2 at floor 6, destination: 2
Elevator 2 arrived at floor 2 — boarding/unboarding for 10s
Elevator 2 debarked passengers at floor 2
```
