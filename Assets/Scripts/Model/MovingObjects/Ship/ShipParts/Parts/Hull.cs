namespace Model.MovingObjects.Ship.ShipParts.Parts
{
    public class Hull : ShipPart
    {
        private new void Awake()
        {
            base.Awake();
            IsAlive = true;
        }
    }
}