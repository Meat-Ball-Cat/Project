using UnityEngine;

internal class PlayerShipBuilder : ShipBuilder
{
    public PlayerShipBuilder(GameObject owner) : base(owner) {}

    public Ship GetShip()
    {
        return AddShip();
    }
}
