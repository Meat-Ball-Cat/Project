namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    internal class Engine : ShipPart, IMoving, ITurning
    {
        public float MovementSpeed => 3;
        public float TurningSpeed => 0.2f;

        private new void Awake()
        {
            base.Awake();
            IsAlive = true;
        }
    }
}