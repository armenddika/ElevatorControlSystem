namespace ElevatorControlSystem.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; } = 1;
        public Direction Direction { get; set; } = Direction.Idle;

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

        public void MoveOneStep()
        {
            if (_stops.Count == 0)
            {
                Direction = Direction.Idle;
                return;
            }

            int target = _stops.Peek();

            if (CurrentFloor < target) CurrentFloor++;
            else if (CurrentFloor > target) CurrentFloor--;

            if (CurrentFloor == target)
            {
                _stops.Dequeue();

                var pickup = _pickupRequests.FirstOrDefault(p => p.FromFloor == CurrentFloor);
                if (pickup != null)
                {
                    _pickupRequests.Dequeue();
                    if (!_stops.Contains(pickup.ToFloor))
                        _stops.Enqueue(pickup.ToFloor);
                }

                UpdateDirection();
            }
        }

        public void UpdateDirection()
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
