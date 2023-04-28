using UnityEngine;
using UnityEngine.Serialization;

public abstract class MovingObject : MonoBehaviour, IManagedObject
{
    protected new Rigidbody2D Rigidbody;
    protected Vector2 CurrentMoveForce;
    protected float CurrentTurnForce;

    [FormerlySerializedAs("_movementSpeed")] [SerializeField]
    protected float movementSpeed;

    [FormerlySerializedAs("_turningSpeed")] [SerializeField]
    protected float turningSpeed;


    protected void Awake()
    {
        if (!gameObject.TryGetComponent(out Rigidbody))
            Rigidbody = gameObject.AddComponent<Rigidbody2D>();
    }


    protected void FixedUpdate()
    {
        Rigidbody.AddRelativeForce(CurrentMoveForce);
        Rigidbody.AddTorque(CurrentTurnForce, ForceMode2D.Force);
    }

    public void Move(Vector2 direction)
    {
        CurrentMoveForce = movementSpeed * direction;
    }

    public void Turn(float axis)
    {
        CurrentTurnForce = turningSpeed * axis;
    }
}
