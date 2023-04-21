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

    public void AddPart(PartShip newPart, Vector2 position)
    {
        newPart.gameObject.transform.SetParent(_shipObject.transform);
        newPart.gameObject.transform.position = position;
        newPart.gameObject.layer = _shipObject.layer;

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
}