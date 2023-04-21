using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class MovingObject : MonoBehaviour, IManagedObject
{
    protected new Rigidbody2D rigidbody;
    protected Vector2 curentMoveForce;
    protected float curentTurningForce;

    [SerializeField]
    protected float _movementSpeed;

    [SerializeField]
    protected float _turningSpeed;


    protected void Awake()
    {
        if (!gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
            rigidbody = gameObject.AddComponent<Rigidbody2D>();
    }


    protected void FixedUpdate()
    {
        rigidbody.AddRelativeForce(curentMoveForce);
        rigidbody.AddTorque(curentTurningForce, ForceMode2D.Force);
    }

    public void Move(Vector2 direction)
    {
        curentMoveForce = _movementSpeed * direction;
    }

    public void Turn(float axis)
    {
        curentTurningForce = _turningSpeed * axis;
    }
}
