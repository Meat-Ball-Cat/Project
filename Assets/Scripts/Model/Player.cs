﻿using Model.MovingObjects.Ship;
using UnityEngine;

namespace Model
{
    internal class Player : MonoBehaviour
    {
        private Ship _ship;
        private Controls _input;

        private void Awake()
        {
            var rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.angularDrag = 1;
            rigidbody.drag = 1;
            rigidbody.gravityScale = 0;
            rigidbody.useAutoMass = true;

            _ship = new PlayerShipFactory(gameObject).GetShip();
        
            _input = new Controls();

            _input.PlayerShip.Move.performed += context 
                => _ship.Move(context.ReadValue<Vector2>());
            _input.PlayerShip.Move.canceled += _ 
                => _ship.Move(Vector2.zero);

            _input.PlayerShip.Turn.performed += context 
                => _ship.Turn(context.ReadValue<float>());
            _input.PlayerShip.Turn.canceled += _ 
                => _ship.Turn(0);

            _input.PlayerShip.Light.canceled += _ 
                => _ship.LightEnabled = !_ship.LightEnabled;

            _input.PlayerShip.Ascend.performed += 
                _ => _ship.AscendOneLevel();

            _input.PlayerShip.Descend.performed +=
                _ => _ship.DescendOneLevel();

        }

        private void OnEnable() => _input?.Enable();

        private void OnDisable() => _input?.Disable();
    }
}