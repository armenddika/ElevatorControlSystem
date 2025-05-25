namespace ElevatorControlSystem.Models
{
    public class ElevatorRequest(int fromFloor, int toFloor)
    {
        public int FromFloor { get; } = fromFloor;
        public int ToFloor { get; } = toFloor;

        public override string ToString() => $"From {FromFloor} to {ToFloor}";
    }
}