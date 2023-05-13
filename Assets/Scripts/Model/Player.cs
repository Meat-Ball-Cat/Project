using System;
using Assets.Scripts.Model.Levels;
using Model.MovingObjects;
using Model.MovingObjects.Ship;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Model
{
    public class Player : MonoBehaviour
    {
        public Ship Ship { get; private set; }
        private Controls _input;

        private void Awake()
        {
            var rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.angularDrag = 1;
            rigidbody.drag = 1;
            rigidbody.gravityScale = 0;
            rigidbody.useAutoMass = true;

            Ship = new PlayerShipFactory(gameObject).GetShip();
            LayerManager.Instance.AddObject(Ship.gameObject);
            Ship.ChangeLayer += (obj, arg) =>
                LayerManager.Instance.SetCurrentLayer(((MovingObject)obj).gameObject.layer);
        
            _input = new Controls();

            _input.PlayerShip.Move.performed += context 
                => Ship.Move(context.ReadValue<Vector2>());
            _input.PlayerShip.Move.canceled += _ 
                => Ship.Move(Vector2.zero);

            _input.PlayerShip.Turn.performed += context 
                => Ship.Turn(context.ReadValue<float>());
            _input.PlayerShip.Turn.canceled += _ 
                => Ship.Turn(0);

            _input.PlayerShip.Light.canceled += _ 
                => Ship.LightEnabled = !Ship.LightEnabled;

            _input.PlayerShip.Ascend.performed += 
                _ => Ship.Ascend();

            _input.PlayerShip.Descend.performed +=
                _ => Ship.Descend();
        }

        public float CurrentShipDepth
            => Ship.transform.position.z;

        private void OnEnable() => _input?.Enable();

        private void OnDisable() => _input?.Disable();
    }
}