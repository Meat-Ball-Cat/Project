namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    internal class Engine : ShipPart, IMoving, ITurning
    {
        public float MovementSpeed => 5;
        public float TurningSpeed => 2f;

        private new void Awake()
        {
            base.Awake();
            IsAlive = true;
        }
    }
}