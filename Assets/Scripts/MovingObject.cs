using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class MovingObject : MonoBehaviour, IManagedObject
{
    protected new Rigidbody2D rigidbody;
    protected Vector2 curentSpeed;
    protected float curentRotation;

    [SerializeField]
    protected float _movementSpeed = 5f;

    [SerializeField]
    protected float _rotationSpeed = 0.1f;


    protected void Awake()
    {
        if (!gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
            rigidbody = gameObject.AddComponent<Rigidbody2D>();
    }


    protected void FixedUpdate()
    {
        rigidbody.AddRelativeForce(curentSpeed);
        rigidbody.AddTorque(curentRotation);
    }

    public void Move(Vector2 direction)
    {
        curentSpeed = _movementSpeed * direction;
    }

    public void Turn(float axis)
    {
        curentRotation = _rotationSpeed * axis;
    }
}
