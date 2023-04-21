using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
internal class Ship : MovingObject
{
    private GameObject _shipObject;

    private Cabin _cabin;

    private readonly PartShip[,] _partPositions = new PartShip[100, 100];
    private PartShip PartInPosition(int x, int y) => _partPositions[x + 50, y + 50];
    private void SetPartInPosition(PartShip part, int x, int y) => _partPositions[x + 50, y + 50] = part;

    private bool _lightEnabled = true;
    public bool LightEnabled
    {
        get => _lightEnabled;
        set
        {
            _lightEnabled = value;
            foreach (var partLight in GetPart(_cabin, false).OfType<ILight>())
                partLight.SetLightEnabled(_lightEnabled);
        }
    }

    private new void Awake()
    {
        base.Awake();

        _shipObject = new GameObject("Ship");
        _shipObject.transform.SetParent(gameObject.transform); // Добавить корабль в качестве дочернего объекта к владельцу корабля
        _shipObject.layer = gameObject.layer;
    }

    /// <summary>
    /// Добавляет часть корабля к кораблю
    /// </summary>
    /// <param name="newPart">Часть, которую надо добавить</param>
    /// <param name="position">Позиция, на которую надо установить данную часть</param>
    /// <exception cref="ArgumentException">В случае, если установка невозможна</exception>
    /// P.S. Возможно стоит переписать через возврат результата установки
    /// А еще надо отрефакторить и вынести вспомогательные методы
    public void AddPart(PartShip newPart, Vector2Int position)
    {
        if (_cabin != null && newPart is Cabin)
            throw new ArgumentException();

        for (var i = 0; i < newPart.Width; i++)
        for (var j = 0; j < newPart.Height; j++)
        {
            if (PartInPosition(position.x + i, position.y + j) != null)
                throw new ArgumentException();
        }

        if (newPart is Cabin cabin)
            _cabin = cabin;

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
        newPart.Died += (obj, e)
            => UpdateMovementSpeed();
        newPart.Died += (obj, e)
            => UpdateTurningSpeed();

        UpdateMovementSpeed();
        UpdateTurningSpeed();
        ActivateShipParts();
    }

    /// <summary>
    /// Обработка события смерти одной из частей корабля
    /// </summary>
    /// <param name="sender">Часть коробля</param>
    /// <param name="e">Ничего</param>
    /// <exception cref="ArgumentException">В случае если sender не является PartShip</exception>
    /// P.S. В этом методе убиваются ненужные объекты, тем самым вызывается событие Die у этого объекта
    /// Каждый раз в обработчике строится обходится граф, что не очень, но эта штука вызывается не часто
    /// так что пока сойдет
    public void Die(object sender, EventArgs e)
    {
        if (sender is Cabin)
        {
            foreach (var part in GetPart(_cabin, true))
                part.Die();
            return;
        }

        if (sender is not PartShip diedPart) throw new ArgumentException();
        var diedParts = GetPart(diedPart, true);
        diedParts.ExceptWith(GetPart(_cabin, false));
        foreach (var part in diedParts.Where(part => part.Alive))
            part.Die();
    }

    /// <summary>
    /// Обновляет текущую максимальную скорость, основываясь на свойствах живых частей коробля
    /// </summary>
    private void UpdateMovementSpeed()
    {
        _movementSpeed = GetPart(_cabin, false)
            .OfType<IMoving>()
            .Select(part => part.MovementSpeed)
            .Sum();
    }

    /// <summary>
    /// Обновляет текущую скорость поворота, основываясь на свойствах живых частей коробля
    /// </summary>
    private void UpdateTurningSpeed()
    {
        _turningSpeed = GetPart(_cabin, false)
            .OfType<ITurning>()
            .Select(part => part.TurningSpeed)
            .Sum();
    }

    private void ActivateShipParts()
    {
        foreach (var connectedPart in GetPart(_cabin, true))
            connectedPart.Alive = true;
    }

    // Создать хэлпер и утащить туда
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

    // Это тоже можно в хэлпер
    // Сделано кривовато
    public static ISet<PartShip> GetPart(PartShip start, bool goOffPart)
    {
        var parts = new HashSet<PartShip>();
        var partQueue = new Queue<PartShip>();

        if (start == null) return parts;

        partQueue.Enqueue(start);

        while (partQueue.Count > 0)
        {
            var currentPart = partQueue.Dequeue();
            if (!goOffPart && !currentPart.Alive) continue;
            if (parts.Contains(currentPart)) continue;
            parts.Add(currentPart);
            foreach (var part in currentPart.ConnectedParts)
                partQueue.Enqueue(part);
        }

        return parts;
    }
}