﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model.MovingObjects.Ship.ShipParts;
using Model.MovingObjects.Ship.ShipParts.Parts;
using UnityEngine;

namespace Model.MovingObjects.Ship
{
    public class Ship : MovingObject
    {
        private GameObject _shipObject;

        private Cockpit _cockpit;

        private readonly ShipPart[,] _partPositions = new ShipPart[100, 100];

        private ShipPart PartInPosition(int x, int y)
            => _partPositions[x + 50, y + 50];

        private void SetPartInPosition(ShipPart shipPart, int x, int y) 
            => _partPositions[x + 50, y + 50] = shipPart;

        private bool _lightEnabled = true;

        public bool LightEnabled
        {
            get
                => _lightEnabled;
            set
            {
                if (value && transform.position.z == 0)
                    return;
                _lightEnabled = value;
                foreach (var partLight in FindJoinedParts(_cockpit, false).OfType<ILight>())
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

        private void Update()
        {
            if (LightEnabled && transform.position.z == 0)
                LightEnabled = false;
            else
                LightEnabled = true;
        }

        /// <summary>
        /// Добавляет часть корабля к кораблю
        /// </summary>
        /// <param name="newShipPart">Часть, которую надо добавить</param>
        /// <param name="position">Позиция, на которую надо установить данную часть</param>
        /// <exception cref="ArgumentException">В случае, если установка невозможна</exception>
        /// P.S. Возможно стоит переписать через возврат результата установки
        /// А еще надо отрефакторить и вынести вспомогательные методы
        public void AddPart(ShipPart newShipPart, Vector2Int position)
        {
            if (_cockpit != null && newShipPart is Cockpit)
                throw new ArgumentException();

            for (var i = 0; i < newShipPart.Width; i++)
            for (var j = 0; j < newShipPart.Height; j++)
            {
                if (PartInPosition(position.x + i, position.y + j) != null)
                    throw new ArgumentException();
            }

            if (newShipPart is Cockpit cabin)
                _cockpit = cabin;

            for (var i = 0; i < newShipPart.Width; i++)
            for (var j = 0; j < newShipPart.Height; j++)
                SetPartInPosition(newShipPart, position.x + i, position.y + j);


            foreach (var pos in GetFrame(position, newShipPart.Width, newShipPart.Height))
            {
                var connectedPart = PartInPosition(pos.x, pos.y);
                if (connectedPart is null)
                    continue;

                connectedPart.ConnectedParts.Add(newShipPart);
                newShipPart.ConnectedParts.Add(connectedPart);
                //Debug.Log(newShipPart + " - " + connectedPart);
            }


            GameObject o;
            (o = newShipPart.gameObject).transform.SetParent(_shipObject.transform);
            o.transform.position = (Vector2)position;
            o.layer = _shipObject.layer;

            newShipPart.Died += Die;
            newShipPart.Died += (_, _)
                => UpdateMovementSpeed();
            newShipPart.Died += (_, _)
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
        /// <exception cref="ArgumentException">В случае если sender не является ShipPart</exception>
        /// P.S. В этом методе убиваются ненужные объекты, тем самым вызывается событие Die у этого объекта
        /// Каждый раз в обработчике строится обходится граф, что не очень, но эта штука вызывается не часто
        /// так что пока сойдет
        private void Die(object sender, EventArgs e)
        {
            if (sender is Cockpit)
            {
                foreach (var part in FindJoinedParts(_cockpit, true))
                    part.Die();
                return;
            }

            if (sender is not ShipPart diedPart)
                throw new ArgumentException();

            var diedParts = FindJoinedParts(diedPart, true);
            diedParts.ExceptWith(FindJoinedParts(_cockpit, false));
            foreach (var part in diedParts.Where(part => part.IsAlive))
                part.Die();
        }

        /// <summary>
        /// Обновляет текущую максимальную скорость, основываясь на свойствах живых частей коробля
        /// </summary>
        private void UpdateMovementSpeed()
        {
            movementSpeed = FindJoinedParts(_cockpit, false)
                .OfType<IMoving>()
                .Select(part => part.MovementSpeed)
                .Sum();
        }

        /// <summary>
        /// Обновляет текущую скорость поворота, основываясь на свойствах живых частей коробля
        /// </summary>
        private void UpdateTurningSpeed()
        {
            turningSpeed = FindJoinedParts(_cockpit, false)
                .OfType<ITurning>()
                .Select(part => part.TurningSpeed)
                .Sum();
        }

        private void ActivateShipParts()
        {
            foreach (var connectedPart in FindJoinedParts(_cockpit, true))
                connectedPart.IsAlive = true;
        }

        // Создать хэлпер и утащить туда
        private static IEnumerable<Vector2Int> GetFrame(Vector2Int position, int width, int height)
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
        private static HashSet<ShipPart> FindJoinedParts(ShipPart start, bool goOffPart)
        {
            var parts = new HashSet<ShipPart>();
            var partQueue = new Queue<ShipPart>();

            if (start == null)
                return parts;

            partQueue.Enqueue(start);

            while (partQueue.Count > 0)
            {
                var currentPart = partQueue.Dequeue();
                if (!goOffPart && !currentPart.IsAlive)
                    continue;
                if (parts.Contains(currentPart))
                    continue;

                parts.Add(currentPart);
                foreach (var part in currentPart.ConnectedParts)
                    partQueue.Enqueue(part);
            }

            return parts;
        }

        public void Shoot()
        {
            foreach (var turret in FindJoinedParts(_cockpit, false).OfType<Turret>())
                turret.Shoot();
        }
    }
}