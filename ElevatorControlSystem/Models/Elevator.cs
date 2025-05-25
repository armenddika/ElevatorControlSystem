namespace ElevatorControlSystem.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; } = 1;
        public Direction Direction { get; private set; } = Direction.Idle;

        private readonly Queue<int> _stops = new();
        private readonly Queue<ElevatorRequest> _pickupRequests = new();

        public Elevator(int id) => Id = id;

        public IReadOnlyCollection<int> Stops => _stops.ToArray();

        public void AddPickupRequest(ElevatorRequest request)
        {
            if (!_stops.Contains(request.FromFloor))
                _stops.Enqueue(request.FromFloor);

            _pickupRequests.Enqueue(request);
            UpdateDirection();
        }

        public void MoveOneStep(int moveSeconds, int boardSeconds)
        {
            if (_stops.Count == 0)
            {
                Direction = Direction.Idle;
                return;
            }

            int target = _stops.Peek();

            if (CurrentFloor < target) CurrentFloor++;
            else if (CurrentFloor > target) CurrentFloor--;

            Thread.Sleep(moveSeconds * 1000);

            if (CurrentFloor == target)
            {
                Console.WriteLine($"Elevator {Id} arrived at floor {CurrentFloor} — boarding/unboarding for {boardSeconds}s");
                Thread.Sleep(boardSeconds * 1000);
                Console.WriteLine($"Elevator {Id} debarked passengers at floor {CurrentFloor}");

                _stops.Dequeue();

                var pickup = _pickupRequests.FirstOrDefault(p => p.FromFloor == CurrentFloor);
                if (pickup != null)
                {
                    Console.WriteLine($"Passenger boarded elevator {Id} at floor {CurrentFloor}, destination: {pickup.ToFloor}");
                    _pickupRequests.Dequeue();
                    if (!_stops.Contains(pickup.ToFloor))
                        _stops.Enqueue(pickup.ToFloor);
                }

                UpdateDirection();
            }
        }

        private void UpdateDirection()
        {
            if (_stops.Count == 0)
            {
                Direction = Direction.Idle;
                return;
            }

            int next = _stops.Peek();
            Direction = next > CurrentFloor ? Direction.Up : Direction.Down;
        }

        public override string ToString()
            => $"Elevator {Id}: floor {CurrentFloor}, direction {Direction}, stops [{string.Join(",", _stops)}]";
    }
}
