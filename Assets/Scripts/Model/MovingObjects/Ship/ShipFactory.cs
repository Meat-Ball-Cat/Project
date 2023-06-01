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
            var ship = _owner.AddComponent<Ship>();


            ship.AddPart(Parts(typeof(Cockpit)), Vector2Int.zero);
            ship.AddPart(Parts(typeof(Spotlight)), new Vector2Int(1, 1));
            ship.AddPart(Parts(typeof(Spotlight)), new Vector2Int(1, 0));
            ship.AddPart(Parts(typeof(Engine)), new Vector2Int(-1, 0));
            ship.AddPart(Parts(typeof(Engine)), new Vector2Int(-1, 1));
            ship.AddPart(Parts(typeof(Turret)), new Vector2Int(1, 2));
            ship.AddPart(Parts(typeof(Turret)), new Vector2Int(-1, 2));

            return ship;
        }

        /// <summary>
        /// TST
        /// HCH
        ///  H 
        /// EEE
        /// </summary>
        /// <param name="shipTemplate"></param>
        /// <returns></returns>
        public Ship FromText(string shipTemplate)
        {
            var ship = _owner.AddComponent<Ship>();
            var lines = shipTemplate.Split('\n');

            var offsetX = lines.Length / 2;

            var hadCockpit = false;
            
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    var partType = lines[i][j] switch
                    {
                        'T' => typeof(Turret),
                        'S' => typeof(Spotlight),
                        'E' => typeof(Engine),
                        'C' => typeof(Cockpit),
                        'H' => typeof(Hull),
                        ' ' => null,
                        _ => throw new ArgumentException($"Cannot build a ship from template: {shipTemplate}"),
                    };
                    
                    if (partType is null)
                        continue;

                    if (partType == typeof(Cockpit))
                        hadCockpit = true;

                    var offsetY = lines[i].Length / 2;
                    ship.AddPart(Parts(partType), new Vector2Int(j - offsetX, i - offsetY) * (-1));
                }
            }

            if (!hadCockpit)
                throw new ArgumentException("The given ship template has no cockpit");

            return ship;
        }

        private static readonly Dictionary<Type, string> PrefabNames = new()
        {
            { typeof(Cockpit), "Cockpit" },
            { typeof(Spotlight), "Spotlight" },
            { typeof(Engine), "Engine" },
            { typeof(Turret), "Turret" },
            { typeof(Hull), "Hull" },
        };

        private static ShipPart Parts(Type partType)
        {
            var fileName = $"Prefabs/ShipParts/{PrefabNames[partType]}";
            return Object.Instantiate(Resources.Load<GameObject>(fileName)).GetComponent<ShipPart>();
        }
    }
}