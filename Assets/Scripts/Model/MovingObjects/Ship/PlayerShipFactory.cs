using UnityEngine;

internal class PlayerShipFactory : ShipFactory
{
    public PlayerShipFactory(GameObject owner) : base(owner) {}

    public Ship GetShip()
    {
        return CreateShip();
    }
}
