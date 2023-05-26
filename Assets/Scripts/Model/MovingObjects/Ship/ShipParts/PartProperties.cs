namespace Model.MovingObjects.Ship.ShipParts
{
    internal interface ILight
    {
        public void SetLightEnabled(bool enabled);
    }

    internal interface IMoving
    {
        public float MovementSpeed { get; }
    }

    internal interface ITurning
    {
        public float TurningSpeed { get; }
    }

    internal interface IShooting
    {
        public void Shoot();
    }
}