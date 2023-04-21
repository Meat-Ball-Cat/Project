using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

internal class ShipBuilder
{
    private readonly GameObject _owner;


    public ShipBuilder(GameObject owner)
    {
        _owner = owner;
    }

    public Ship AddShip()
    {
        var ship = _owner.AddComponent<Ship>();


        ship.AddPart(CreatePartShip(typeof(Cabin)), Vector2Int.zero);
        ship.AddPart(CreatePartShip(typeof(SpotLight)), new Vector2Int(1, 1));

        return ship;
    }

    private static PartShip CreatePartShip(Type partType)
    {
        if (!partType.IsSubclassOf(typeof(PartShip)))
            throw new ArgumentException();

        var childObject = new GameObject();
        childObject.AddComponent<BoxCollider2D>();
        var renderer = childObject.AddComponent<SpriteRenderer>();
        var part = childObject.AddComponent(partType) as PartShip;

        renderer.sprite = CreateSprite(Texture[part.GetType()], part.Width, part.Height);

        return part;
    }

    private static Sprite CreateSprite(Texture2D texture, int width = 1, int height = 1)
    {
        return Sprite.Create(texture, 
            new Rect(0, 0, texture.width, texture.height), 
            Vector2.one * 0.5f, 
            Math.Max((float)texture.width / width, (float)texture.height / height));
    }

    private static readonly Dictionary<Type, Texture2D> Texture = new()
    {
        { typeof(Cabin), CreateTexture(Color.blue) },
        { typeof(SpotLight), CreateTexture(Color.yellow) }
    };

    private static Texture2D CreateTexture(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixels(new [] { color });
        texture.Apply();
        return texture;
    }
}