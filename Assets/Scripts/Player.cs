using UnityEngine;
using UnityEngine.Windows;

internal class Player : MonoBehaviour
{
    private IManagedObject _ship;
    private Controls _input;

    private void Awake()
    {
        _ship = new PlayerShipBuilder(gameObject).GetShip();
        
        
        _input = new Controls();

        _input.PlayerShip.Move.performed += context => _ship.Move(context.ReadValue<Vector2>());
        _input.PlayerShip.Move.canceled += _ => _ship.Move(Vector2.zero);

        _input.PlayerShip.Rotation.performed += context => _ship.Turn(context.ReadValue<float>());
        _input.PlayerShip.Rotation.canceled += _ => _ship.Turn(0);
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input.Disable();
}