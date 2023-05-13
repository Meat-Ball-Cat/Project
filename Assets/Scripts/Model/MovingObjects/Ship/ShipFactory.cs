using System;
using System.Collections.Generic;
using Model.MovingObjects.Ship.ShipParts;
using Model.MovingObjects.Ship.ShipParts.Parts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Model.MovingObjects.Ship
{
    internal class ShipFactory
    {
        private readonly GameObject _owner;


        public ShipFactory(GameObject owner)
        {
            _owner = owner;
        }

        public Ship CreateShip()
        {
            var ship = _owner.AddComponent<global::Model.MovingObjects.Ship.Ship>();


            ship.AddPart(Parts(typeof(Cockpit)), Vector2Int.zero);
            ship.AddPart(Parts(typeof(Spotlight)), new Vector2Int(1, 1));
            ship.AddPart(Parts(typeof(Spotlight)), new Vector2Int(1, 0));
            ship.AddPart(Parts(typeof(Engine)), new Vector2Int(-1, 0));
            ship.AddPart(Parts(typeof(Engine)), new Vector2Int(-1, 1));

            return ship;
        }

        private static readonly Dictionary<Type, string> PrefabNames = new()
        {
            { typeof(Cockpit), "Cockpit" },
            { typeof(Spotlight), "Spotlight" },
            { typeof(Engine), "Engine" },
        };

        private static ShipPart Parts(Type partType)
        {
            var fileName = PrefabNames[partType];
            return Object.Instantiate(Resources.Load<GameObject>(fileName)).GetComponent<ShipPart>();
        }

    }
}