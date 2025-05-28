using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Services
{
    public class ElevatorController
    {
        public int TotalFloors { get; }
        private readonly List<Elevator> _elevators;

        public IReadOnlyList<Elevator> Elevators => _elevators;

        public ElevatorController(int elevatorCount, int totalFloors)
        {
            TotalFloors = totalFloors;
            _elevators = Enumerable.Range(1, elevatorCount)
                .Select(id => new Elevator(id))
                .ToList();
        }

        public void RequestElevators(IEnumerable<ElevatorRequest> requests)
        {
            Console.WriteLine("Processing multiple requests simultaneously:");
            foreach (var request in requests)
            {
                Console.WriteLine($"Request: From Floor {request.FromFloor} to Floor {request.ToFloor}");
            }

            foreach (var request in requests)
            {
                RequestElevator(request);
            }
        }

        public void RequestElevator(ElevatorRequest request)
        {
            Console.WriteLine($"Request received: From Floor {request.FromFloor} to Floor {request.ToFloor}");

            var direction = request.ToFloor > request.FromFloor ? Direction.Up : Direction.Down;

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
                var idleElevators = _elevators.Where(e => e.Direction == Direction.Idle).ToList();

                if (idleElevators.Any())
                {
                    selected = idleElevators
                        .OrderBy(e => Math.Abs(e.CurrentFloor - request.FromFloor))
                        .First();
                }
                else
                {
                    selected = _elevators
                        .OrderBy(e => e.Stops.Count)
                        .ThenBy(e => Math.Abs(e.CurrentFloor - request.FromFloor))
                        .First();
                }
            }

            Console.WriteLine($"Elevator {selected.Id} assigned to request: From Floor {request.FromFloor} to Floor {request.ToFloor}");
            selected.AddPickupRequest(request);
        }

        public void StepAll()
        {
            foreach (var e in _elevators)
            {
                e.MoveOneStep();
                Console.WriteLine($"Elevator {e.Id} moved to Floor {e.CurrentFloor}, Direction: {e.Direction}");
            }
        }

        public void PrintStatus()
        {
            foreach (var e in _elevators)
                Console.WriteLine(e);
        }
    }
}