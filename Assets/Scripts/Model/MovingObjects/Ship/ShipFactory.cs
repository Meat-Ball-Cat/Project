using System;
using System.Collections.Generic;
using Model.MovingObjects.Ship.ShipParts;
using Model.MovingObjects.Ship.ShipParts.Parts;
using UnityEngine;

namespace Model.MovingObjects.Ship
{
    internal class ShipFactory
    {
        private readonly GameObject _owner;


        public ShipFactory(GameObject owner)
        {
            _owner = owner;
        }

        public global::Model.MovingObjects.Ship.Ship CreateShip()
        {
            var ship = _owner.AddComponent<global::Model.MovingObjects.Ship.Ship>();


            ship.AddPart(CreateShipPart(typeof(Cockpit)), Vector2Int.zero);
            ship.AddPart(CreateShipPart(typeof(Spotlight)), new Vector2Int(1, 1));
            ship.AddPart(CreateShipPart(typeof(Spotlight)), new Vector2Int(1, 0));

            return ship;
        }

        private static ShipPart CreateShipPart(Type partType)
        {
            if (!partType.IsSubclassOf(typeof(ShipPart)))
                throw new ArgumentException();

            var childObject = new GameObject();
            childObject.AddComponent<BoxCollider2D>();
            var renderer = childObject.AddComponent<SpriteRenderer>();
            var part = childObject.AddComponent(partType) as ShipPart;

            renderer.sprite = CreateSprite(Textures[part.GetType()], part.Width, part.Height);

            return part;
        }

        private static Sprite CreateSprite(Texture2D texture, int width = 1, int height = 1)
        {
            return Sprite.Create(texture, 
                new Rect(0, 0, texture.width, texture.height), 
                Vector2.one * 0.5f, 
                Math.Max((float)texture.width / width, (float)texture.height / height));
        }

        private static readonly Dictionary<Type, Texture2D> Textures = new()
        {
            { typeof(Cockpit), CreateTexture(Color.blue) },
            { typeof(Spotlight), CreateTexture(Color.yellow) }
        };

        private static Texture2D CreateTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixels(new [] { color });
            texture.Apply();
            return texture;
        }
    }
}