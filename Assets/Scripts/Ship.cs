using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
internal class Ship : MovingObject
{
    private GameObject _shipObject;
    private bool _lightEnabled = true;

    private List<PartShip> _parts;

    private readonly PartShip[,] _partPositions = new PartShip[100, 100];
    private PartShip PartInPosition(int x, int y) => _partPositions[x + 50, y + 50];
    private void SetPartInPosition(PartShip part, int x, int y) => _partPositions[x + 50, y + 50] = part;

    private bool LightEnabled
    {
        get => _lightEnabled;
        set
        {
            _lightEnabled = value;
            foreach (var partLight in _parts.OfType<ILight>())
                partLight.SetLightEnabled(_lightEnabled);
        }
    }

    private new void Awake()
    {
        base.Awake();

        _shipObject = new GameObject("Ship");
        _shipObject.transform.SetParent(gameObject.transform); // Добавить корабль в качестве дочернего объекта к владельцу корабля
        _shipObject.layer = gameObject.layer;

        _parts = new List<PartShip>();
    }

    public void AddPart(PartShip newPart, Vector2Int position)
    {
        for (var i = 0; i < newPart.Width; i++)
        for (var j = 0; j < newPart.Height; j++)
        {
            if (PartInPosition(position.x + i, position.y + j) != null)
                throw new ArgumentException();
        }

        for (var i = 0; i < newPart.Width; i++)
        for (var j = 0; j < newPart.Height; j++)
        {
            SetPartInPosition(newPart, position.x + i, position.y + j);
        }

        foreach (var pos in GetFrame(position, newPart.Width, newPart.Height))
        {
            var connectedPart = PartInPosition(pos.x, pos.y);
            if (connectedPart is null) continue;
            connectedPart.ConnectedParts.Add(newPart);
            newPart.ConnectedParts.Add(connectedPart);
            Debug.Log(newPart + " - " + connectedPart);
        }


        newPart.gameObject.transform.SetParent(_shipObject.transform);
        newPart.gameObject.transform.position = (Vector2)position;
        newPart.gameObject.layer = _shipObject.layer;

        newPart.Died += Die;

        if (newPart is IMoving moving)
            _movementSpeed += moving.MovementSpeed;

        if (newPart is ITurning turning)
            _turningSpeed += turning.TurningSpeed;

        _parts.Add(newPart);
    }

    public void Light()
    {
        LightEnabled = !LightEnabled;
    }

    public void Die(object sender, EventArgs e)
    {
        if (sender is Cabin)
            foreach (var part in _parts)
            {
                if (!part.Alive) continue;

                part.Die();
            }
    }

    public static IEnumerable<Vector2Int> GetFrame(Vector2Int position, int width, int height)
    {
        for (var i = 0; i < width; i++)
        {
            yield return position + new Vector2Int(i, -1);
            yield return position + new Vector2Int(i, height);
        }

        for (var i = 0; i < height; i++)
        {
            yield return position + new Vector2Int(-1, i);
            yield return position + new Vector2Int(width, i);
        }

    }
}