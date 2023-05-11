using UnityEngine;

namespace Model.MovingObjects
{
    internal interface IManagedObject
    {
        public void Move(Vector2 direction);
        public void Turn(float side);
    }
}
