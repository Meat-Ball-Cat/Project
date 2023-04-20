using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

internal class ShipBuilder
{
    private GameObject _owner;

    public ShipBuilder(GameObject owner)
    {
        _owner = owner;
    }

    public Ship AddShip()
    {
        var ship = _owner.AddComponent<Ship>();
        

        var shipObject = new GameObject("Ship");
        shipObject.transform.SetParent(_owner.transform); // Добавить корабль в качестве дочернего объекта к владельцу корабля
        shipObject.layer = _owner.layer;

        var childObject = new GameObject("Square");
        #region test ship part

        // Добавим спрайт квадарата с колайдером как тело нашего корабля

        var spriteSize = 64;
        var texture = new Texture2D(spriteSize, spriteSize);
        texture.SetPixels(Enumerable.Range(0, texture.width * texture.height).Select(i => Color.white).ToArray());
        texture.Apply();

        var squareSprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
        
        // Присвоить дочернему объекту компонент SpriteRenderer и назначить ему созданный спрайт
        var renderer = childObject.AddComponent<SpriteRenderer>();
        renderer.sprite = squareSprite;

        #endregion
        childObject.transform.SetParent(shipObject.transform); // Добавить новый объект в качестве дочернего объекта к кораблю
        childObject.layer = shipObject.layer;
        childObject.AddComponent<BoxCollider2D>();

        return ship;
    }
}