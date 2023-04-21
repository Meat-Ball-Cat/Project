using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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


        ship.AddPart(CreateCabin(), Vector2.zero);
        ship.AddPart(CreateSpotLight(), new Vector2(1, 1));

        return ship;
    }

    private Cabin CreateCabin()
    {
        var childObject = new GameObject("Cabin");
        var renderer = childObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSprite(Color.blue);
        childObject.AddComponent<BoxCollider2D>();
        var part = childObject.AddComponent<Cabin>();

        return part;
    }

    private SpotLight CreateSpotLight()
    {
        var childObject = new GameObject("Cabin");
        var renderer = childObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSprite(Color.white);
        childObject.AddComponent<BoxCollider2D>();
        var part = childObject.AddComponent<SpotLight>();

        return part;
    }

    private static Sprite CreateSprite(Color color, int width = 1, int height = 1)
    {
        const int spriteSize = 100;
        var texture = new Texture2D(spriteSize * width, spriteSize * height);
        texture.SetPixels(Enumerable.Range(0, texture.width * texture.height)
            .Select(_ => color)
            .ToArray());
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }
}