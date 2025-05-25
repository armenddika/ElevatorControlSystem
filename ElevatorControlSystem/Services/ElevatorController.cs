using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Services
{
    public class ElevatorController
    {
        public int TotalFloors { get; }
        private readonly List<Elevator> _elevators;
        private const int MoveTime = 10;
        private const int BoardTime = 10;

        public IReadOnlyList<Elevator> Elevators => _elevators;

        public ElevatorController(int elevatorCount, int totalFloors)
        {
            TotalFloors = totalFloors;
            _elevators = Enumerable.Range(1, elevatorCount)
                .Select(id => new Elevator(id))
                .ToList();
        }

        public void RequestElevator(ElevatorRequest request)
        {
            var direction = request.ToFloor > request.FromFloor ? Direction.Up : Direction.Down;

            // 1. Elevators going the same direction and will pass the pickup floor
            var matchingElevators = _elevators.Where(e =>
                e.Direction == direction &&
                ((direction == Direction.Up && e.CurrentFloor <= request.FromFloor) ||
                 (direction == Direction.Down && e.CurrentFloor >= request.FromFloor))
            ).ToList();

            Elevator selected;

            if (matchingElevators.Any())
            {
                selected = matchingElevators
                    .OrderBy(e => Math.Abs(e.CurrentFloor - request.FromFloor))
                    .First();
            }
            else
            {
                // 2. Try idle elevators
                var idleElevators = _elevators.Where(e => e.Direction == Direction.Idle).ToList();

                if (idleElevators.Any())
                {
                    selected = idleElevators
                        .OrderBy(e => Math.Abs(e.CurrentFloor - request.FromFloor))
                        .First();
                }
                else
                {
                    // 3. Fallback: pick elevator with fewest stops, then closest
                    selected = _elevators
                        .OrderBy(e => e.Stops.Count)
                        .ThenBy(e => Math.Abs(e.CurrentFloor - request.FromFloor))
                        .First();
                }
            }

            Console.WriteLine($"Request: Passenger at floor {request.FromFloor} → destination {request.ToFloor} assigned to Elevator {selected.Id}");
            selected.AddPickupRequest(request);
        }


        public void StepAll()
        {
            foreach (var e in _elevators)
                e.MoveOneStep(MoveTime, BoardTime);
        }

        public void PrintStatus()
        {
            foreach (var e in _elevators)
                Console.WriteLine(e);
        }
    }
}